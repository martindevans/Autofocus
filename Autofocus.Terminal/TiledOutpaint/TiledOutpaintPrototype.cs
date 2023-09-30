using Autofocus.Config;
using Autofocus.ImageSharp.Extensions;

namespace Autofocus.Terminal.TiledOutpaint;

public class TiledOutpaintPrototype
{
    public async Task Run()
    {
        var api = new StableDiffusion();
        var sampler = await api.Sampler("DDIM");
        var model = await api.StableDiffusionModel("cardosAnime_v20");

        var prompt = new PromptConfig
        {
            Positive = "1girl, backpack, outdoors, mountains, sunny, frilled_skirt, glasses, looking_at_viewer, short_hair, short_sleeves, skirt, smile, solo, standing, thighhighs",
            Negative = "easynegative, badhandv4, nsfw",
        };

        var txt2img = await api.TextToImage(
            new()
            {
                Seed = 16,

                Prompt = prompt,

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

                ClipSkip = 2,
            }
        );

        var img = txt2img.Images[0];
        await (await img.ToImageSharpAsync()).SaveAsPngAsync("Input.png");

        var iresult = await api.Interrogate(new InterrogateConfig
        {
            Image = img,
            Model = InterrogateModel.DeepDanbooru
        });
        Console.WriteLine(iresult.Caption);

        var input = await Image.LoadAsync<Rgba32>("Input.png");
        var outpainter = new TiledOutpaint(api, model, sampler, 2, 2, 75);

        prompt.Positive += ", ancient buildings";

        var counter = 0;
        var results = await outpainter.Outpaint(prompt, input);
        foreach (var result in results)
        {
            using var imgi = await result.ToImageSharpAsync();
            await imgi.SaveAsPngAsync($"output_{counter++}.png");
        }
    }
}