using Autofocus.Config;
using Autofocus.FeatureRepaint.Extensions;
using Autofocus.ImageSharp;
using Autofocus.ImageSharp.Extensions;
using Autofocus.Models;
using FaceAiSharp;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;

namespace Autofocus.FeatureRepaint
{
    public class FeatureRepainter
    {
        private readonly IStableDiffusion _api;
        private readonly IStableDiffusionModel _model;
        private readonly SamplerConfig _sampler;
        private readonly LoraConfig[] _loras;

        private readonly IFaceDetectorWithLandmarks _detector;

        public FeatureRepainter(IStableDiffusion api, IStableDiffusionModel model, SamplerConfig sampler, LoraConfig[]? loras = null)
        {
            _api = api;
            _model = model;
            _sampler = sampler;
            _loras = loras ?? [];

            _detector = FaceAiSharpBundleFactory.CreateFaceDetectorWithLandmarks();
        }

        /// <summary>
        /// Analyse an image to find faces
        /// </summary>
        /// <param name="img"></param>
        /// <param name="config"></param>
        /// <returns></returns>
        public async Task<AnalysisResult> Analyse(Image img, AnalysisConfig config)
        {
            if (img is Image<Rgb24> cast)
            {
                return await Analyse(cast, config);
            }
            else
            {
                using var img24 = img.CloneAs<Rgb24>();
                return await Analyse(img24, config);
            }
        }

        /// <summary>
        /// Analyse an image to find faces
        /// </summary>
        /// <param name="img"></param>
        /// <param name="config"></param>
        /// <returns></returns>
        public Task<AnalysisResult> Analyse(Image<Rgb24> img, AnalysisConfig config)
        {
            return Task.Run(() =>
            {
                // Get set of detections, sorted by area ascending
                var detections = _detector.DetectFaces(img);
                var filtered = (from face in detections
                                where face.Confidence.HasValue && face.Confidence.Value > config.MinConfidence
                                where face.Box.Width >= config.MinSize.Width
                                where face.Box.Height >= config.MinSize.Height
                                where face.Box.Width / img.Width > config.MinFactor.Width
                                where face.Box.Height / img.Height > config.MinFactor.Height
                                orderby face.Box.Width * face.Box.Height ascending
                                select face).ToList();

                // Remove smallest detections until we have the number we want
                while (filtered.Count > config.MaxDetections)
                    filtered.RemoveAt(0);

                // Convert to detections
                var faces = from item in filtered
                            select new FaceDetection(
                                ToIntRect(item.Box),
                                item.Landmarks == null ? null : _detector.GetLeftEyeCenter(item.Landmarks),
                                item.Landmarks == null ? null : _detector.GetRightEyeCenter(item.Landmarks)
                            );

                // Return results
                return new AnalysisResult(faces);
            });

            static Rectangle ToIntRect(RectangleF rect)
            {
                return new Rectangle((int)rect.X, (int)rect.Y, (int)rect.Width, (int)rect.Height);
            }
        }

        /// <summary>
        /// Repaint faces in the order they are in the analysis results. If there are more faces than prompts, the last prompt will be reused
        /// </summary>
        /// <param name="input"></param>
        /// <param name="analysis"></param>
        /// <param name="prompts"></param>
        /// <param name="blend">How many pixels to expand, use this space for blending</param>
        /// <param name="strength"></param>
        public async Task<Image> Repaint(Image input, AnalysisResult analysis, IReadOnlyList<FacePrompt> prompts, ushort blend = 32, double strength = 0.6)
        {
            if (prompts.Count == 0)
                throw new ArgumentException("Must supply at least one prompt", nameof(prompts));

            var output = input.CloneAs<Rgb24>();
            using var input24 = input.CloneAs<Rgb24>();

            for (var i = 0; i < analysis.Faces.Count; i++)
            {
                var face = analysis.Faces[i];

                var prompt = i >= prompts.Count ? prompts[^1] : prompts[i];
                var promptFace = string.Join(", ", prompt.Face, prompt.Eyes);

                var faceBound = face.Bounds;
                faceBound.Inflate(blend, blend);
                var subbox = FitAspectBoxFlexible(faceBound, input.Bounds.AspectRatio(), input.Bounds, out var altRatio);

                using var faceBox = input24.Clone(ctx =>
                {
                    ctx.Crop(subbox);

                    if (altRatio)
                        ctx.Resize(input.Height, input.Width);
                    else
                        ctx.Resize(input.Width, input.Height);
                });

                using var mask = MaskHelper.CreateEllipseBlurOutlineMask(faceBox.Width, faceBox.Height, blend);

                var img2img = await _api.Image2Image(
                    new()
                    {
                        Images = { await faceBox.ToAutofocusImageAsync() },
                        Model = _model,
                        Seed = -1,
                        Sampler = _sampler,
                        BatchSize = 1,
                        Batches = 1,

                        DenoisingStrength = strength,

                        Prompt = new()
                        {
                            Positive = promptFace,
                            Negative = prompt.Negative,
                        },

                        Mask = await mask.ToAutofocusImageAsync(),

                        Lora = _loras,
                    }
                );

                // Get result from img2img
                using var img2imgResult = await img2img.Images[0].ToImageSharpAsync<Rgba32>();

                // Shrink down to size
                img2imgResult.Mutate(ctx =>
                {
                    ctx.Resize(subbox.Width, subbox.Height);
                });

                // Copy mask into alpha channel
                MaskHelper.ApplyMaskAsAlpha(img2imgResult, mask);

                // Blend into output
                output.Mutate(ctx =>
                {
                    // ReSharper disable once AccessToDisposedClosure
                    ctx.DrawImage(img2imgResult, new Point(subbox.X, subbox.Y), new GraphicsOptions
                    {
                        ColorBlendingMode = PixelColorBlendingMode.Normal
                    });
                });
            }

            return output;
        }

        private static Rectangle FitAspectBoxFlexible(Rectangle detection, float aspectRatio, Rectangle imageBounds, out bool altRatio)
        {
            // Try both orientations
            var best = FitOne(detection, aspectRatio, imageBounds);
            var alt = FitOne(detection, 1f / aspectRatio, imageBounds);

            // Pick the one that expanded the least area from the detection
            var detArea = detection.Width * detection.Height;
            var bestArea = best.Width * best.Height;
            var altArea = alt.Width * alt.Height;

            altRatio = (altArea - detArea) < (bestArea - detArea);

            return altRatio ? alt : best;
        }

        /// <summary>
        /// Fit a box around another box, without overhanging an outer bounds
        /// </summary>
        /// <param name="inner">The inner box we must encompass</param>
        /// <param name="ratio">The aspect ratio of the encompassing box (width / height)</param>
        /// <param name="outer">The outer bounds, final result must fit within this</param>
        /// <returns>A rectangle that encompasses inner as much as possible while obeying ratio and outer</returns>
        private static Rectangle FitOne(Rectangle inner, float ratio, Rectangle outer)
        {
            // Create a box around "inner" with the right aspect ratio
            float targetWidth = inner.Width;
            float targetHeight = inner.Height;

            // Determine if we need to expand width or height to meet the ratio while encompassing 'inner'
            if (inner.AspectRatio() > ratio)
                targetHeight = targetWidth / ratio;
            else
                targetWidth = targetHeight * ratio;

            // Ensure the fit box is smaller than the entire outer box
            if (targetWidth > outer.Width)
            {
                targetWidth = outer.Width;
                targetHeight = targetWidth / ratio;
            }
            if (targetHeight > outer.Height)
            {
                targetHeight = outer.Height;
                targetWidth = targetHeight * ratio;
            }

            // Calculate initial position: Centered on the "inner" box
            var x = inner.X + (inner.Width - targetWidth) / 2f;
            var y = inner.Y + (inner.Height - targetHeight) / 2f;

            // Translate to fix any overhangs out of the outer box
            if (x < outer.Left)
                x = outer.Left;
            if (x + targetWidth > outer.Right)
                x = outer.Right - targetWidth;

            if (y < outer.Top)
                y = outer.Top;
            if (y + targetHeight > outer.Bottom)
                y = outer.Bottom - targetHeight;

            // Final result cast back to integer Rectangle
            return new Rectangle(
                (int)Math.Round(x),
                (int)Math.Round(y),
                (int)Math.Round(targetWidth),
                (int)Math.Round(targetHeight)
            );
        }
    }

    /// <summary>
    /// Configuration for image analysis
    /// </summary>
    public record AnalysisConfig
    {
        /// <summary>
        /// The minimum size (in pixels) of detected faces
        /// </summary>
        public (uint Width, uint Height) MinSize { get; init; } = (0, 0);

        /// <summary>
        /// The minimum size (as a factor of the image size) of detects faces
        /// </summary>
        public (float Width, float Height) MinFactor { get; init; } = (0, 0);

        /// <summary>
        /// Maximum number of face detections, they will be trimmed by area (removing smallest first)
        /// </summary>
        public uint MaxDetections { get; init; } = int.MaxValue;

        /// <summary>
        /// Minimum confidence of face detections
        /// </summary>
        public float MinConfidence { get; init; }
    }

    /// <summary>
    /// Results from image analysis
    /// </summary>
    public record AnalysisResult
    {
        private readonly FaceDetection[] _faces;

        /// <summary>
        /// The detected faces
        /// </summary>
        public IReadOnlyList<FaceDetection> Faces => _faces;

        public AnalysisResult(IEnumerable<FaceDetection> faces)
        {
            _faces = faces.ToArray();
        }

        public void LeftToRight()
        {
            Array.Sort(
                _faces,
                (a, b) => a.Bounds.Left.CompareTo(b.Bounds.Left)
            );
        }

        public void RightToLeft()
        {
            Array.Sort(
                _faces,
                (a, b) => -a.Bounds.Right.CompareTo(b.Bounds.Right)
            );
        }

        public void SizeDescending()
        {
            Array.Sort(
                _faces,
                (a, b) => -(a.Bounds.Width * a.Bounds.Height).CompareTo(b.Bounds.Width * b.Bounds.Height)
            );
        }
    }

    /// <summary>
    /// A single face detection results
    /// </summary>
    /// <param name="Bounds"></param>
    /// <param name="LeftEye"></param>
    /// <param name="RightEye"></param>
    public record FaceDetection(Rectangle Bounds, PointF? LeftEye, PointF? RightEye)
    {
        public float GetEyeRadius()
        {
            return Math.Max(
                Bounds.Width * 0.15f,
                Bounds.Height * 0.1f
            );
        }
    }

    /// <summary>
    /// A prompt to re-paint a face
    /// </summary>
    /// <param name="Face"></param>
    /// <param name="Eyes"></param>
    /// <param name="Negative"></param>
    public record FacePrompt(string Face, string Eyes, string Negative);
}
