using Autofocus;
using Autofocus.Models;
using SixLabors.ImageSharp;

var api = new StableDiffusion();

var sampler  = await api.Sampler("DPM++ SDE");
var model    = await api.StableDiffusionModel("cardosAnime_v20");
var upscaler = await api.Upscaler("Lanczos") ?? throw new NotImplementedException("no upscaler");
var style = (await api.Styles()).FirstOrDefault(a => a.Name.Equals("BWphoto")) ?? throw new NotImplementedException("no style");

var response = await api.TextToImage(
    new TextToImageConfig
    {
        Seed = 16,
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

Console.WriteLine("# Finished");
Console.WriteLine($" + {response.Parameters.Prompt}");
Console.WriteLine($" - {response.Parameters.NegativePrompt}");
Console.WriteLine();

for (var i = 0; i < response.Images.Count; i++)
    response.Images[i].ToImage().SaveAsPng($"image{i}.png");

Console.WriteLine("# PngInfo");
var info = await api.PngInfo(response.Images[0]);
Console.WriteLine(info.Info);