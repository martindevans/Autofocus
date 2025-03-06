using Autofocus.Config;
using Spectre.Console;

namespace Autofocus.Terminal;

public class Queueing
{
    public async Task Run()
    {
        var api = new StableDiffusion();
        await api.Ping();

        var model = await api.StableDiffusionModel("cardosAnime_v20");
        var sampler = await api.Sampler("UniPC");

        var tasks = new List<Task>();

        for (var i = 0; i < 10; i++)
        {
            var task = api.TextToImage(new()
            {
                Prompt = new()
                {
                    Positive = "mountains, trees, birds, sky, clouds, landscape",
                    Negative = "easynegative, 1girl, 1boy, people",
                },
                Seed = 1234,
                Sampler = new()
                {
                    Sampler = sampler,
                    SamplingSteps = 20
                },
                Model = model,
                Width = 512,
                Height = 512,
                BatchSize = 4,
                TaskId = $"Autofocus{i}",
            });
            tasks.Add(task);
        }

        for (var i = 0; i < 10; i++)
        {
            var pending = await api.PendingTasks();
            Console.WriteLine(pending.Tasks.Count);
            await Task.Delay(1000);
        }

        await Task.WhenAll(tasks);
    }
}