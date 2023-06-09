using Autofocus;
using Autofocus.Config;
using Autofocus.Terminal.Extensions;

var api = new StableDiffusion();
var cnet = await api.TryGetControlNet() ?? throw new NotImplementedException("no controlnet!");

var model = await api.StableDiffusionModel("cardosAnime_v20");
var sampler = await api.Sampler("DPM++ SDE");

var txt2img = await api.TextToImage(
    new()
    {
        Seed = 22,

        Prompt = new()
        {
            Positive = "1girl, backpack, outdoors, mountains, sunny, frilled_skirt, glasses, looking_at_viewer, short_hair, short_sleeves, skirt, smile, solo, standing, thighhighs",
            Negative = "easynegative, badhandv4, nsfw",
            Styles = {
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

        ControlNet = new()
        {
            Image = Image.Load("standing_06.png").ToEncodedImage(),
            Model = await cnet.Model("control_v11p_sd15_openpose [cab727d4]")
        },
    }
);

for (var i = 0; i < txt2img.Images.Count; i++)
{
    txt2img.Images[i].ToImage().SaveAsPng($"txt2img_image{i}.png");

    var interrogate = await api.Interrogate(new InterrogateConfig
    {
        Image = txt2img.Images[i],
        Model = InterrogateModel.DeepDanbooru
    });
    Console.WriteLine(interrogate.Caption);
}

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
            Positive = "1boy, (adult), backpack, outdoors, mountains, sunny, glasses, looking_at_viewer, short_hair, short_sleeves, smile, solo, standing, (standing_on_one:1.25)",
            Negative = "easynegative, badhandv4, nsfw, child",
            Styles =
            {
                await api.Style("TellTale")
            }
        },

        Seed = new()
        {
            Seed = 22,
        },

        Sampler = new()
        {
            Sampler = sampler,
            SamplingSteps = 20,
        },
    }
);

for (var i = 0; i < img2img.Images.Count; i++)
    img2img.Images[i].ToImage().SaveAsPng($"img2img_image{i}.png");

Console.WriteLine();
Console.WriteLine("Starting ControlNet");

var cnetResults = await cnet.Preprocess(
    new ControlNetPreprocessConfig()
    {
        Images = txt2img.Images.ToList(),
        Module = await cnet.Module("canny"),
    }
);

for (var i = 0; i < cnetResults.Images.Count; i++)
    cnetResults.Images[i].ToImage().SaveAsPng($"cnet_image{i}.png");