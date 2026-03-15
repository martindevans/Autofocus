using Autofocus.ImageSharp.Extensions;
using SixLabors.ImageSharp;

namespace Autofocus.Terminal;

public class StableDiffusionCpp
{
    public async Task Run(StableDiffusion api)
    {
        await api.Ping();
        await api.Progress(true);

        var model = await api.StableDiffusionModel("waiANIPONYXL_v140_q4k");

        var sampler = await api.Sampler("lcm");
        var scheduler = await api.Scheduler("lcm");
        var lora = await api.Lora("lcm_sdxl");

        var txt2img = await api.TextToImage(
            new()
            {
                Seed = 22,

                Prompt = new()
                {
                    Positive = "1girl, backpack, outdoors, mountains, sunny, frilled_skirt, glasses, looking_at_viewer, short_hair, short_sleeves, skirt, smile, solo, standing, thighhighs",
                    Negative = "easynegative, badhandv4, nsfw",
                },

                Sampler = new()
                {
                    Sampler = sampler,
                    SamplingSteps = 10,
                    Scheduler = scheduler,
                    CfgScale = 1.75f
                },

                Model = model,
                BatchSize = 1,
                Batches = 1,
                Width = 1216,
                Height = 832,

                Lora = [
                    new(lora),
                ],

                AdditionalScripts = {
                    null
                },
            }
        );

        for (var i = 0; i < txt2img.Images.Count; i++)
            await (await txt2img.Images[i].ToImageSharpAsync()).SaveAsPngAsync($"sdcpp_txt2img_image{i}.png");
    }
}