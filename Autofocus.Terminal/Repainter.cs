using Autofocus.Config;
using Autofocus.FeatureRepaint;
using Autofocus.ImageSharp.Extensions;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Drawing;
using SixLabors.ImageSharp.Drawing.Processing;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;

namespace Autofocus.Terminal;

public class Repainter
{
    public async Task Run(IStableDiffusion api)
    {
        await api.Ping();

        var model = await api.StableDiffusionModel("waiANIPONYXL_v140_q4k");
        var sampler = new SamplerConfig()
        {
            Sampler = await api.Sampler("lcm"),
            Scheduler = await api.Scheduler("lcm"),
            SamplingSteps = 7,
            CfgScale = 1.75f
        };

        // Generate the input image
        var txt2img = await api.TextToImage(
            new()
            {
                Seed = 23,
                Prompt = new()
                {
                    Positive = "2girls, backpack, outdoors, mountains, sunny, looking at viewer, short hair, short sleeves, skirt, smile, solo, standing, thighhighs",
                    Negative = "easynegative, badhandv4, nsfw",
                },
                Sampler = sampler,
                Model = model,
                BatchSize = 1,
                Batches = 1,
                Width = 1216,
                Height = 832,

                Lora = [
                    new(await api.Lora("lcm_sdxl")),
                ],

                AdditionalScripts = {
                    null
                },
            }
        );

        // Save to disk
        using var input = await txt2img.Images[0].ToImageSharpAsync();
        await input.SaveAsPngAsync("repainter_input.png");

        // Find features
        var repainter = new FeatureRepainter(api, model, sampler, [ new(await api.Lora("lcm_sdxl")) ]);
        var analysis = await repainter.Analyse(input, new AnalysisConfig());

        // Draw face boxes
        using var boxes = input.CloneAs<Rgb24>();
        boxes.Mutate(ctx =>
        {
            foreach (var result in analysis.Faces)
            {
                ctx.Draw(
                    new SolidPen(Color.Red, 3),
                    result.Bounds
                );

                if (result.LeftEye.HasValue)
                {
                    ctx.Fill(
                        Color.LimeGreen,
                        new EllipsePolygon(result.LeftEye.Value, result.GetEyeRadius())
                    );
                }

                if (result.RightEye.HasValue)
                {
                    ctx.Fill(
                        Color.LimeGreen,
                        new EllipsePolygon(result.RightEye.Value, result.GetEyeRadius())
                    );
                }
            }
        });
        await boxes.SaveAsPngAsync("repainter_detections.png");

        // Repaint
        analysis.LeftToRight();
        var output = await repainter.Repaint(
            input,
            analysis,
            prompts: [
                new("1girl, (((glasses))), looking at viewer, short hair, smiling", "green eyes", "easynegative"),
                new("1girl, looking at viewer, short hair, ((grinning))", "red eyes, blue eyes, heterochromia", "easynegative, (((glasses)))"),
            ]
        );

        await output.SaveAsPngAsync("repainter_result.png");
    }
}