using Autofocus;
using Autofocus.Models;
using SixLabors.ImageSharp;

var api = new StableDiffusion();

var sampler = await api.Sampler("DPM++ SDE");
var model   = await api.StableDiffusionModel("cardosAnime_v20");

var response = await api.TextToImage(
    new TextToImageConfig
    {
        Prompt = "masterpiece, sharp, A starry sky",
        NegativePrompt = "sunny, people, person, 1girl, 1boy",
        Seed = 16,
        Sampler = sampler,
        SamplingSteps = 10,
        Model = model,

        BatchSize = 2,
        Batches = 1,
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