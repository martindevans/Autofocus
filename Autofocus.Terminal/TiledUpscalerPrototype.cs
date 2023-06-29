using Autofocus.ImageSharp.Extensions;

namespace Autofocus.Terminal;

public class TiledUpscalerPrototype
{
    private readonly string[] _args;

    public TiledUpscalerPrototype(string[] args)
    {
        _args = args;
    }

    public async Task Run()
    {
        var api = new StableDiffusion();
        var sampler = await api.Sampler("DPM++ SDE");
        var model = await api.StableDiffusionModel("cardosAnime_v20");

        if (!File.Exists("Input.png"))
        {
            var txt2img = await api.TextToImage(
                new()
                {
                    Seed = 22,

                    Prompt = new()
                    {
                        Positive = "1girl, backpack, outdoors, mountains, sunny, frilled_skirt, glasses, looking_at_viewer, short_hair, short_sleeves, skirt, smile, solo, standing, thighhighs",
                        Negative = "easynegative, badhandv4, nsfw",
                        Styles =
                        {
                            await api.Style("BWphoto"),
                        }
                    },

                    Sampler = new()
                    {
                        Sampler = sampler,
                        SamplingSteps = 20,
                    },

                    Model = model,
                    BatchSize = 1,
                    Batches = 1,
                    RestoreFaces = false,
                    Height = 512,
                    Width = 512,
                }
            );

            var img = txt2img.Images[0];
            await (await img.ToImageSharpAsync()).SaveAsPngAsync("Input.png");
        }

        var input = await Image.LoadAsync("Input.png");
        var upscaler = new TiledUpscaler.TiledUpscaler(api, model, sampler);
        var result = await upscaler.Upscale(input, 2000, 2000);
        await result.SaveAsPngAsync("output.png");
    }
}