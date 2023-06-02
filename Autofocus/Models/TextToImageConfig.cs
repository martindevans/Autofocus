using System.Text.Json.Serialization;

#pragma warning disable IDE0044
#pragma warning disable IDE0052
// ReSharper disable CollectionNeverQueried.Local
// ReSharper disable FieldCanBeMadeReadOnly.Local
// ReSharper disable NotAccessedField.Local

namespace Autofocus.Models;

public record TextToImageConfig
{
    public required string Prompt { get; init; }
    public required string NegativePrompt { get; init; }

    public uint? Width { get; set; }
    public uint? Height { get; set; }

    public int? Seed { get; set; }
    public int? SubSeed { get; set; }
    public float? SubseedStrength { get; set; }

    public ISampler? Sampler { get; set; }
    public int? SamplingSteps { get; set; }

    public IStableDiffusionModel? Model { get; set; }

    public int? BatchSize { get; set; }
    public int? Batches { get; set; }

    //"enable_hr": false,
    //"denoising_strength": 0,
    //"firstphase_width": 0,
    //"firstphase_height": 0,
    //"hr_scale": 2,
    //"hr_upscaler": "string",
    //"hr_second_pass_steps": 0,
    //"hr_resize_x": 0,
    //"hr_resize_y": 0,
    //"styles": [
    //"string"
    //    ],
    //"seed_resize_from_h": -1,
    //"seed_resize_from_w": -1,
    //"cfg_scale": 7,
    //"restore_faces": false,
    //"tiling": false,
    //"do_not_save_samples": false,
    //"do_not_save_grid": false,
    //"negative_prompt": "string",
    //"eta": 0,
    //"s_min_uncond": 0,
    //"s_churn": 0,
    //"s_tmax": 0,
    //"s_tmin": 0,
    //"s_noise": 1,
    //"override_settings": {},
    //"override_settings_restore_afterwards": true,
    //"script_args": [],
    //"sampler_index": "Euler",
    //"script_name": "string",
    //"send_images": true,
    //"save_images": false,
    //"alwayson_scripts": {}
}

internal class TextToImageConfigRequest
{
    [JsonPropertyName("prompt")] public string Prompt { get; init; }
    [JsonPropertyName("negative_prompt")] public string NegativePrompt { get; init; }

    [JsonPropertyName("height"), JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public uint? Width { get; init; }

    [JsonPropertyName("width"), JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public uint? Height { get; init; }

    [JsonPropertyName("seed"), JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public int? Seed { get; init; }

    [JsonPropertyName("subseed"), JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public int? SubSeed { get; init; }

    [JsonPropertyName("subseed_strength"), JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public float? SubSeedStrength { get; init; }

    [JsonPropertyName("steps"), JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public int? SamplingSteps { get; init; }

    [JsonPropertyName("sampler_name"), JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? SamplerName { get; init; }

    [JsonPropertyName("n_iter"), JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public int? Batches { get; init; }

    [JsonPropertyName("batch_size"), JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public int? BatcheSize { get; init; }

    [JsonPropertyName("override_settings")]
    public Dictionary<string, string> OverrideSettings = new();

    public TextToImageConfigRequest(TextToImageConfig config)
    {
        
        Prompt = config.Prompt;
        NegativePrompt = config.NegativePrompt;
        Height = config.Height;
        Width = config.Width;
        Seed = config.Seed;
        SubSeed = config.SubSeed;
        SubSeedStrength = config.SubseedStrength;
        SamplingSteps = config.SamplingSteps;
        SamplerName = config.Sampler?.Name;
        Batches = config.Batches;
        BatcheSize = config.BatchSize;

        var model = config.Model?.Title;
        if (model != null)
            OverrideSettings.Add("sd_model_checkpoint", model);
    }
}