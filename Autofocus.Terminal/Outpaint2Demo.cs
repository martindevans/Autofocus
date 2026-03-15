using Autofocus.Config;
using Autofocus.ImageSharp.Extensions;
using Autofocus.Outpaint;
using SixLabors.ImageSharp;

namespace Autofocus.Terminal;

public class Outpaint2Demo
{
    public async Task Run(StableDiffusion api)
    {
        await api.Ping();

        var genPrompt = new PromptConfig
        {
            Positive = "1girl, white hair, purple eyes, black choker, slight smile, mountains, clouds, sky",
            Negative = "easynegative, badhandv4",
        };

        var model = await api.StableDiffusionModel("cardosAnime_v20");
        var config = new TextToImageConfig
        {
            Seed = new(),

            Prompt = genPrompt,

            Sampler = new()
            {
                Sampler = await api.Sampler("dpm++2m"),
                Scheduler = await api.Scheduler("karras"),
                SamplingSteps = 20,
            },

            Model = model,
            BatchSize = 1,
            Batches = 1,
            RestoreFaces = false,
            Height = 512,
            Width = 512,
        };

        if (!File.Exists("txt2img_image.png"))
        {
            // Generate initial image
            var txt2img = await api.TextToImage(config);
            await (await txt2img.Images.Single().ToImageSharpAsync()).SaveAsPngAsync("txt2img_image.png");
        }
        var startFrame = await Image.LoadAsync("txt2img_image.png");

        // outpainting prompt
        var outpaintPrompt = genPrompt;

        // Outpaint
        var outpainter = new TwoStepOutpainter(api, model, new SamplerConfig
        {
            Sampler = await api.Sampler("dpm++2m"),
            Scheduler = await api.Scheduler("karras"),
        })
        {
            UseControlNetTile = false,
        };
        var results = await outpainter.Outpaint(outpaintPrompt, startFrame, Console.WriteLine);
        for (var i = 0; i < results.Count; i++)
            await (await results[i].ToImageSharpAsync()).SaveAsPngAsync($"outpaint_{i}.png");
    }
}