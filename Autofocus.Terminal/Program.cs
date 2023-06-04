using Autofocus;
using Autofocus.Terminal.Extensions;

var api = new StableDiffusion();

var model = await api.StableDiffusionModel("cardosAnime_v20");
var sampler = await api.Sampler("DPM++ SDE");

Console.WriteLine("Generating");
var txt2img = await api.TextToImage(
    new()
    {
        Seed = new()
        {
            Seed = 16
        },

        Prompt = new()
        {
            Positive = "1girl, backpack, outdoors, mountains, sunny, frilled_skirt, glasses, looking_at_viewer, short_hair, short_sleeves, skirt, smile, solo, standing, (standing_on_one:1.25), thighhighs",
            Negative = "easynegative, badhandv4, nsfw",
            Styles = {
                await api.Style("BWphoto"),
            }
        },

        Sampler = new()
        {
            Sampler = sampler,
            SamplingSteps = 10,
        },

        Model = model,
        BatchSize = 1,
        Batches = 1,
        RestoreFaces = true,
        Height = 512,
        Width = 512,

        //HighRes = new()
        //{
        //    DenoisingStrength = 0.5,
        //    Upscaler = await api.Upscaler("Lanczos"),

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

        Model = model,

        Prompt = new()
        {
            Positive = "1boy, (adult), backpack, outdoors, mountains, sunny, glasses, looking_at_viewer, short_hair, short_sleeves, skirt, smile, solo, standing, (standing_on_one:1.25)",
            Negative = "easynegative, badhandv4, nsfw, child",
            Styles =
            {
                await api.Style("TellTale")
            }
        },

        Seed = new()
        {
            Seed = 17,
        },

        Sampler = new()
        {
            Sampler = sampler,
            SamplingSteps = 10,
        },
    }
);

for (var i = 0; i < img2img.Images.Count; i++)
    img2img.Images[i].ToImage().SaveAsPng($"img2img_image{i}.png");