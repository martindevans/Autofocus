using System.Text.Json.Serialization;
using Autofocus.Models;

namespace Autofocus.Config;

public enum InterrogateModel
{
    CLIP,
    DeepDanbooru
}

public class InterrogateConfig
{
    public required Base64EncodedImage Image { get; init; }
    public InterrogateModel Model { get; set; } = InterrogateModel.CLIP;
}

internal class InterrogateConfigRequest
{
    [JsonPropertyName("image")]
    public string Image { get; init; }

    [JsonPropertyName("model")]
    public string Model { get; init; }

    public InterrogateConfigRequest(InterrogateConfig config)
    {
        Image = config.Image.Base64();
        Model = config.Model.ToString().ToLowerInvariant();
    }
}