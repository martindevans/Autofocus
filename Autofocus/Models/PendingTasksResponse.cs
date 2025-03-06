using System.Text.Json.Serialization;

namespace Autofocus.Models;

public interface IPendingTasks
{
    public IReadOnlyList<string> Tasks { get; }
}

internal class PendingTasksResponse
    : IPendingTasks
{
    [JsonPropertyName("size")] public int QueueSize { get; init; }
    [JsonPropertyName("tasks")] public string[] Tasks { get; init; } = [ ];

    IReadOnlyList<string> IPendingTasks.Tasks => Tasks;
}