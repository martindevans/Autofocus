using Autofocus;
using Autofocus.Terminal.Extensions;

var api = new StableDiffusion();

var sampler  = await api.Sampler("DPM++ SDE") ?? throw new InvalidOperationException("no sampler");
var model    = await api.StableDiffusionModel("cardosAnime_v20") ?? throw new InvalidOperationException("no model");
var upscaler = await api.Upscaler("Lanczos") ?? throw new InvalidOperationException("no upscaler");
var style = (await api.Styles()).FirstOrDefault(a => a.Name.Equals("BWphoto")) ?? throw new InvalidOperationException("no style");

Console.WriteLine("Generating");
var txt2img = await api.TextToImage(
    new()
    {
        Seed = new()
        {
            Seed = 16
        },
        Prompt = "1girl, backpack, outdoors, mountains, sunny, frilled_skirt, glasses, looking_at_viewer, short_hair, short_sleeves, skirt, smile, solo, standing, (standing_on_one:1.25), thighhighs",
        NegativePrompt = "easynegative, badhandv4, nsfw",
        Styles = {
            style,
        },

        Sampler = sampler,
        SamplingSteps = 10,
        Model = model,

        BatchSize = 1,
        Batches = 1,

        RestoreFaces = true,

        Height = 512,
        Width = 512,

        //HighRes = new()
        //{
        //    DenoisingStrength = 0.5,
        //    Upscaler = upscaler,

        //    Width = 512,
        //    Height = 512,
        //}
    }
);

for (var i = 0; i < txt2img.Images.Count; i++)
    txt2img.Images[i].ToImage().SaveAsPng($"txt2img_image{i}.png");

Console.WriteLine("# PngInfo");
var info = await api.PngInfo(txt2img.Images[0]);
Console.WriteLine(info.Info);

Console.WriteLine();
Console.WriteLine("Starting Image2Image");

var img2img = await api.Image2Image(
    new()
    {
        Images = {
            txt2img.Images[0]
        },

        Prompt = "1boy, (adult), backpack, outdoors, mountains, sunny, glasses, looking_at_viewer, short_hair, short_sleeves, skirt, smile, solo, standing, (standing_on_one:1.25)",
        NegativePrompt = "easynegative, badhandv4, nsfw, child",

        Seed = new()
        {
            Seed = 17,
        }
    }
);

for (var i = 0; i < img2img.Images.Count; i++)
    img2img.Images[i].ToImage().SaveAsPng($"img2img_image{i}.png");