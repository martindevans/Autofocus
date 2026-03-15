using System.Text.Json.Serialization;

namespace Autofocus.Config;

public record LoraConfig
{
    public LoraConfig(string path, float multiplier = 1.0f, bool isHighNoise = false)
    {
        Path = path;
        Multiplier = multiplier;
        IsHighNoise = isHighNoise;
    }

    [JsonPropertyName("path")]
    public string Path { get; set; }

    [JsonPropertyName("multiplier")]
    public float Multiplier { get; set; }

    [JsonPropertyName("is_high_noise")]
    public bool IsHighNoise { get; set; }
}