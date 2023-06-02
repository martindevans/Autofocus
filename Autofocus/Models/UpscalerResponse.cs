using System.Text.Json.Serialization;

namespace Autofocus.Models;

public interface IUpscaler
{
    public string Name { get; }
    public string ModelName { get; }
    public string ModelPath { get; }
    public string ModelUrl { get; }
    public double Scale { get; }
}

internal class UpscalerResponse
    : IUpscaler
{
    [JsonPropertyName("name")] public string Name { get; init; } 
    [JsonPropertyName("model_name")] public string ModelName { get; init; } 
    [JsonPropertyName("model_path")] public string ModelPath { get; init; } 
    [JsonPropertyName("model_url")] public string ModelUrl { get; init; } 
    [JsonPropertyName("scale")] public double Scale { get; init; }
}