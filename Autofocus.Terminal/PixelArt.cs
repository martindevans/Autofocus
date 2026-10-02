using Autofocus.Config;
using Autofocus.PixelArt;
using ImageMagick;
using ImageMagick.Factories;

namespace Autofocus.Terminal;

public class PixelArt
{
    public async Task Run(IStableDiffusion api)
    {
        await api.Ping();

        var model = await api.StableDiffusionModel("prefectPonyXL_v50");
        var sampler = await api.Sampler("UniPC");

        var prompt = new PromptConfig
        {
            Positive = "rating_safe, score_9, score_8_up, score_7_up, 1girl, expressionless, looking at viewer",
            Negative = "easynegative, score_6, score_5, score_4, 1boy",
        };
        
        var initialImage = (await api.TextToImage(new()
        {
            Prompt = prompt,
            Seed = 1234,
            Sampler = new()
            {
                Sampler = sampler,
                SamplingSteps = 20
            },
            Model = model,
            Width = 512,
            Height = 512,
            BatchSize = 1,
        })).Images[0];

        var result = await initialImage.PixelArt(
            new PixelArtConfig
            {
                MaxColors = 128,
                Strength = PixelArtStrength.VeryHigh
            },
            new MagickImageFactory()
        );

        await using (var fs = File.OpenWrite("Start.png"))
            await result.WriteAsync(fs, MagickFormat.Png);
        await using (var fe = File.OpenWrite("End.png"))
            await result.WriteAsync(fe);
        
        RaylibHelpers.ShowPng("End.png");
    }
}