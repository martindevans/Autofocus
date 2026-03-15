using System.Text.Json.Serialization;

namespace Autofocus.Models;

public interface ILora
{
    public string Name { get; }
    internal string Path { get; }
}

internal class LoraResponse
    : ILora
{
    [JsonPropertyName("name")] public string Name { get; init; } = null!;
    [JsonPropertyName("path")] public string Path { get; init; } = null!;
}