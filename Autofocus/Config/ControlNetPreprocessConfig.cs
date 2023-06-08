using System.Text.Json.Serialization;
using Autofocus.CtrlNet;
using Autofocus.Models;

namespace Autofocus.Config;

public class ControlNetPreprocessConfig
{
    public required ControlNetModule Module { get; init; }

    public List<Base64EncodedImage> Images { get; set; } = new();

    public int? Resolution { get; set; }

    public float? ParameterA { get; set; }
    public float? ParameterB { get; set; }
}

internal class ControlNetPreprocessConfigRequest
{
    [JsonPropertyName("controlnet_module")]
    public string ControlNetModule { get; set; }

    [JsonPropertyName("controlnet_input_images")]
    public string[] InputImages { get; set; }

    [JsonPropertyName("controlnet_processor_res"), JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public int? Resolution { get; set; }

    [JsonPropertyName("controlnet_threshold_a"), JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public float? ParameterA { get; set; }

    [JsonPropertyName("controlnet_threshold_b"), JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public float? ParameterB { get; set; }

    public ControlNetPreprocessConfigRequest(ControlNetPreprocessConfig config)
    {
        ControlNetModule = config.Module.Name;
        InputImages = config.Images.Select(img => img.Base64()).ToArray();
        Resolution = config.Resolution;
        ParameterA = config.ParameterA;
        ParameterB = config.ParameterB;

        //todo:validate parameters of module are in range (module_list returns info about sliders, but it's not read yet)
    }
}