using System.Text.Json.Serialization;
using Autofocus.Models;

namespace Autofocus.Config;

public record LoraConfig
{
    public LoraConfig(ILora lora, float multiplier = 1.0f, bool isHighNoise = false)
    {
        Lora = lora;

        Multiplier = multiplier;
        IsHighNoise = isHighNoise;
    }

    public ILora Lora { get; }

    [JsonPropertyName("path")]
    public string Path => Lora.Path;

    [JsonPropertyName("multiplier")]
    public float Multiplier { get; set; }

    [JsonPropertyName("is_high_noise")]
    public bool IsHighNoise { get; set; }
}