using Autofocus.Config;
using Autofocus.ImageSharp.Extensions;
using SixLabors.ImageSharp;

namespace Autofocus.Terminal;

public class PixelArt
{
    public async Task Run()
    {
        var api = new StableDiffusion();
        await api.Ping();

        var model = await api.StableDiffusionModel("prefectPonyXL_v50");
        var sampler = await api.Sampler("UniPC");

        var initialImage = (await api.TextToImage(new()
        {
            Prompt = new()
            {
                Positive = "rating_safe, score_9, score_8_up, score_7_up, (Pixellated, Pixel Art), mountains, trees, birds, sky, clouds, landscape",
                Negative = "easynegative, score_6, score_5, score_4, 1girl, 1boy, people",
            },
            Seed = 4321,
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

        var pixellator = new Autofocus.PixelArt.PixelArt(api, model, sampler);
        var result = await pixellator.Image2Image(new Autofocus.PixelArt.PixelArt.Config()
            {
                Denoising = 0.25,
                Rounds = 1,
                MaxColors = 128,
            },
            prompt: new PromptConfig
            {
                Positive = "((Pixellated, Pixel Art)), mountains, trees, birds, sky, clouds, landscape",
                Negative = "easynegative, 1girl, 1boy, people",
            },
            seed: 4321,
            initialImage,
            progressCallback: _ => { }
        );

        await (await initialImage.ToImageSharpAsync()).SaveAsJpegAsync("Start.jpeg");
        await result.SaveAsJpegAsync("End.jpeg");
    }
}