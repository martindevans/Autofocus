using Autofocus.Config;
using Autofocus.Models;
using SixLabors.ImageSharp.Drawing.Processing;
using Autofocus.ImageSharp.Extensions;

namespace Autofocus.Terminal.TiledOutpaint;

internal class TiledOutpaint
{
    private readonly StableDiffusion _api;
    private readonly IStableDiffusionModel _model;
    private readonly ISampler _sampler;

    private readonly int _batchSize1;
    private readonly int _batchSize2;
    private readonly int _steps1;
    private readonly int _steps2;

    public TiledOutpaint(StableDiffusion api, IStableDiffusionModel model, ISampler sampler, int batchSize1, int batchSize2, int steps)
    {
        _api = api;
        _model = model;
        _sampler = sampler;

        _batchSize1 = batchSize1;
        _steps1 = (int)Math.Ceiling(steps * 0.7);

        _batchSize2 = batchSize2;
        _steps2 = (int)Math.Ceiling(steps * 0.3);
    }

    public async Task<IReadOnlyCollection<Base64EncodedImage>> Outpaint(PromptConfig prompt, Image originalInput)
    {
        return await ExpandOutwards(prompt, originalInput);
    }

    private async Task<IReadOnlyCollection<Base64EncodedImage>> ExpandOutwards(PromptConfig prompt, Image input)
    {
        // Calculate average colour of the whole image
        var average = input.AverageColor();

        var drawingOptions = new GraphicsOptions
        {
            Antialias = true,
            ColorBlendingMode = PixelColorBlendingMode.Normal,
        };

        // Create an image expanded outwards by 128 in all directions
        using var inputImage = new Image<Rgba32>(input.Width + 256, input.Height + 256);
        inputImage.Mutate(ctx =>
        {
            ctx.Fill(average);
            ctx.DrawImage(input, new Point(128, 128), drawingOptions);
        });
        inputImage.Bleed(new Rectangle(128, 128, input.Width - 1, input.Height - 1), 128, null, 1);

        // Create a mask covering the noise with a smooth transition into the image
        using var inputMask = new Image<Rgba32>(inputImage.Width, inputImage.Height);
        inputMask.Mutate(ctx =>
        {
            const int blur = 4;
            var rect = new RectangleF(128 + blur, 128 + blur, input.Width - blur * 2, input.Width - blur * 2);

            var nearBlack = new Color(new Rgba32(4, 4, 4, 255));
            ctx.Fill(Color.White)
               .Fill(nearBlack, rect)
               .BoxBlur(blur)
               .Fill(nearBlack, rect);
        });

        // Reduce image size for initial step
        inputImage.Mutate(ctx => ctx.Resize(input.Size));
        inputMask.Mutate(ctx => ctx.Resize(input.Size));

        var result1 = await _api.Image2Image(new ImageToImageConfig
        {
            Images =
            {
                await inputImage.ToAutofocusImageAsync()
            },

            Mask = await inputMask.ToAutofocusImageAsync(),

            Model = _model,
            DenoisingStrength = 0.75,

            BatchSize = _batchSize1,

            Width = (uint)inputMask.Width,
            Height = (uint)inputMask.Height,

            Prompt = prompt,

            Seed = -1,

            Sampler = new()
            {
                Sampler = _sampler,
                SamplingSteps = _steps1,
                CfgScale = 4
            },

            ClipSkip = 2,
        });

        var results = new List<Base64EncodedImage>();
        foreach (var item in result1.Images)
        {
            // Expand image back up to proper size and draw the original input into the middle
            using var image = await item.ToImageSharpAsync();
            image.Mutate(ctx =>
            {
                ctx.Resize(input.Size + new Size(256, 256));
                ctx.DrawImage(input, new Point(128, 128), drawingOptions);
            });

            var inner = await Redraw(image, prompt, -1);
            results.AddRange(inner);
                
        }
        return results;
    }

    private async Task<IReadOnlyCollection<Base64EncodedImage>> Redraw(Image input, PromptConfig prompt, SeedConfig seed)
    {
        var result2 = await _api.Image2Image(new ImageToImageConfig
        {
            Images =
            {
                await input.ToAutofocusImageAsync(),
            },

            Model = _model,
            DenoisingStrength = 0.25,

            BatchSize = _batchSize2,

            Width = (uint)input.Width,
            Height = (uint)input.Height,

            Prompt = prompt,
            Seed = seed,

            Sampler = new()
            {
                Sampler = _sampler,
                SamplingSteps = _steps2,
                CfgScale = 8
            },

            ClipSkip = 2,
        });

        return result2.Images;
    }
}