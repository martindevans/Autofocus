using System.Text.Json.Serialization;

namespace Autofocus.Models;

internal class InternalProgressRequest
{
    [JsonPropertyName("id_task")] public string Id { get; init; }

    public InternalProgressRequest(string id)
    {
        Id = id;
    }
}

public interface IInternalProgress
{
    public bool Active { get; }
    public bool Queued { get; }
    public bool Completed { get; }
    public double? ETA { get; }
    public string? TextInfo { get; }
}

internal class InternalProgressResponse
    : IInternalProgress
{
    [JsonPropertyName("active")] public bool Active { get; init; }
    [JsonPropertyName("queued")] public bool Queued { get; init; }
    [JsonPropertyName("completed")] public bool Completed { get; init; }
    [JsonPropertyName("eta")] public double? ETA { get; init; }
    [JsonPropertyName("textinfo")] public string? TextInfo { get; init; }
}