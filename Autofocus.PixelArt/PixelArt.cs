using Autofocus.Config;
using Autofocus.ImageSharp.Extensions;
using Autofocus.Models;
using Autofocus.Utilities.Progress;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.ColorSpaces;
using SixLabors.ImageSharp.ColorSpaces.Conversion;
using SixLabors.ImageSharp.Processing;
using SixLabors.ImageSharp.Processing.Processors.Dithering;
using SixLabors.ImageSharp.Processing.Processors.Quantization;
using SixLabors.ImageSharp.Processing.Processors.Transforms;

namespace Autofocus.PixelArt;

public class PixelArt
{
    private readonly IStableDiffusion _api;
    private readonly IStableDiffusionModel _model;
    private readonly ISampler _sampler;

    public PixelArt(IStableDiffusion api, IStableDiffusionModel model, ISampler sampler)
    {
        _api = api;
        _model = model;
        _sampler = sampler;
    }

    public Task<Image> Image2Image(Config config, PromptConfig prompt, SeedConfig seed, Base64EncodedImage input, Action<float> progressCallback)
    {
        return Image2Image(config, prompt, seed, input, report =>
        {
            progressCallback(report.Progress);
            return Task.CompletedTask;
        });
    }

    public async Task<Image> Image2Image(Config config, PromptConfig prompt, SeedConfig seed, Base64EncodedImage input, Func<ProgressReport, Task> progressCallback)
    {
        using var inputImage = await input.ToImageSharpAsync();
        return await Image2Image(config, prompt, seed, inputImage, progressCallback);
    }

    public Task<Image> Image2Image(Config config, PromptConfig prompt, SeedConfig seed, Image input, Action<float> progressCallback)
    {
        return Image2Image(config, prompt, seed, input, report =>
        {
            progressCallback(report.Progress);
            return Task.CompletedTask;
        });
    }

    public async Task<Image> Image2Image(Config config, PromptConfig prompt, SeedConfig seed, Image input, Func<ProgressReport, Task> progressCallback)
    {
        var progress = new ProgressMonitor(_api);
        progress.ProgressEvent += progressCallback;
        await progress.Report(0.01f);

        // Clone input so it is not mutated
        input = input.Clone(ctx => { });

        // Run rounds, reducing denoising each time
        for (var i = 0; i < config.Rounds; i++)
        {
            // Shrink and expand to pixellate
            Crunch(config, input);

            // Redraw the image
            var result = await _api.Image2Image(new ImageToImageConfig
            {
                Prompt = prompt,
                Sampler = new SamplerConfig
                {
                    Sampler = _sampler,
                    SamplingSteps = 25,
                    CfgScale = 8,
                },
                Model = _model,
                Seed = seed with { Seed = seed.Seed + i, },
                DenoisingStrength = double.Lerp(config.Denoising, config.Denoising / 10, ((float)i / config.Rounds)),
                Images = { await input.ToAutofocusImageAsync() }
            });
            input = await result.Images.Single().ToImageSharpAsync();
        }

        // Shrink and expand to pixellate one final time
        Crunch(config, input);

        return input;
    }

    private void Crunch(Config config, Image image)
    {
        image.Mutate(img =>
        {
            var q = new WuQuantizer
            {
                Options =
                {
                    Dither = ErrorDither.FloydSteinberg,
                    MaxColors = config.MaxColors,
                    DitherScale = 1,
                },
            };

            var size = img.GetCurrentSize();
            img.Resize(size / 4, new BoxResampler(), true);
            img.Saturate(1.15f);
            img.Quantize(q);
            img.Resize(size, new NearestNeighborResampler(), true);
        });
    }

    public record Config
    {
        /// <summary>
        /// How many round of image 2 image to run, with resizing in between
        /// </summary>
        public required int Rounds { get; init; }

        /// <summary>
        /// Initial denoising to do
        /// </summary>
        public required double Denoising { get; init; }

        /// <summary>
        /// How many colours to quantize to
        /// </summary>
        public required int MaxColors { get; init; }
    }
}