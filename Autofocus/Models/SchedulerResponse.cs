using System.Text.Json.Serialization;

namespace Autofocus.Models;

public interface IScheduler
{
    public string Name { get; }
}

internal class SchedulerResponse
    : IScheduler
{
    [JsonPropertyName("name")] public string Name { get; init; } = null!;
}