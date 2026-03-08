using Autofocus.ImageSharp.Extensions;
using SixLabors.ImageSharp;

namespace Autofocus.Terminal;

public class StableDiffusionCpp
{
    public async Task Run()
    {
        var api = new StableDiffusion("http://martin-ai-server:1234")
        {
            PingEndpoint = "/",
            EnableProgress = false,
        };

        await api.Ping();
        await api.Progress(true);

        var model = await api.StableDiffusionModel("cardosAnime_v20");

        var sampler = await api.Sampler("Euler_a");
        var scheduler = await api.Scheduler("karras");

        var txt2img = await api.TextToImage(
            new()
            {
                Seed = 22,

                Prompt = new()
                {
                    Positive = "1girl, backpack, outdoors, mountains, sunny, frilled_skirt, glasses, looking_at_viewer, short_hair, short_sleeves, skirt, smile, solo, standing, thighhighs",
                    Negative = "disfigured, bad art, bad quality, low quality, blurry, poorly drawn, mutated, out of frame, bad anatomy, bad anatomy, nsfw",
                },

                Sampler = new()
                {
                    Sampler = sampler,
                    SamplingSteps = 20,
                    Scheduler = scheduler,
                },

                Model = model,
                BatchSize = 4,
                Batches = 1,
                Height = 512,
                Width = 512,

                AdditionalScripts = {
                    null
                },
            }
        );

        for (var i = 0; i < txt2img.Images.Count; i++)
            await (await txt2img.Images[i].ToImageSharpAsync()).SaveAsPngAsync($"txt2img_image{i}.png");
    }
}