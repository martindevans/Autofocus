using System.Text.Json.Serialization;

namespace Autofocus.Models;

public record TextToImageConfig
{
    public required string Prompt { get; init; }
    public required string NegativePrompt { get; init; }
    public List<IPromptStyle> Styles { get; init; } = new();

    public required uint Width { get; set; }
    public required uint Height { get; set; }
    public bool Tiling { get; set; }

    public int? Seed { get; set; }
    public int? SubSeed { get; set; }
    public double? SubseedStrength { get; set; }
    public uint? SeedResizeFromWidth { get; set; }
    public uint? SeedResizeFromHeight { get; set; }

    public ISampler? Sampler { get; set; }
    public int? SamplingSteps { get; set; }
    public double? CfgScale { get; set; }
    public double? Eta { get; set; }

    public IStableDiffusionModel? Model { get; set; }

    public int? BatchSize { get; set; }
    public int? Batches { get; set; }

    public bool RestoreFaces { get; set; }

    public HighResConfig? HighRes { get; set; }

    //"firstphase_width": 0,
    //"firstphase_height": 0,
    //"hr_second_pass_steps": 0,
    //"s_min_uncond": 0,
    //"s_churn": 0,
    //"s_tmax": 0,
    //"s_tmin": 0,
    //"s_noise": 1,
    //"script_args": [],
    //"script_name": "string",
    //"send_images": true,
    //"save_images": false,
    //"do_not_save_samples": false,
    //"do_not_save_grid": false,
    //"alwayson_scripts": {}
}

public record HighResConfig
{
    public required double DenoisingStrength { get; set; }

    public required IUpscaler Upscaler { get; set; }

    public required uint Width { get; set; }
    public required uint Height { get; set; }
}

internal class TextToImageConfigRequest
{
    [JsonPropertyName("prompt")]
    public string Prompt { get; init; }

    [JsonPropertyName("negative_prompt")]
    public string NegativePrompt { get; init; }

    [JsonPropertyName("styles")]
    public string[] Styles { get; init; }

    [JsonPropertyName("width"), JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public uint? Width { get; init; }

    [JsonPropertyName("height"), JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public uint? Height { get; init; }

    [JsonPropertyName("tiling"), JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public bool? Tiling { get; init; }

    [JsonPropertyName("seed"), JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public int? Seed { get; init; }

    [JsonPropertyName("subseed"), JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public int? SubSeed { get; init; }

    [JsonPropertyName("subseed_strength"), JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public float? SubSeedStrength { get; init; }

    [JsonPropertyName("seed_resize_from_w"), JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public uint? SeedResizeFromWidth { get; init; }

    [JsonPropertyName("seed_resize_from_h"), JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public uint? SeedResizeFromHeight { get; init; }

    [JsonPropertyName("steps"), JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public int? SamplingSteps { get; init; }

    [JsonPropertyName("sampler_name"), JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? SamplerName { get; init; }

    [JsonPropertyName("cfg_scale"), JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public float? CfgScale { get; init; }

    [JsonPropertyName("eta"), JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public float? Eta { get; init; }

    [JsonPropertyName("n_iter"), JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public int? Batches { get; init; }

    [JsonPropertyName("batch_size"), JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public int? BatcheSize { get; init; }

    [JsonPropertyName("restore_faces")]
    public bool RestoreFaces { get; init; }


    [JsonPropertyName("enable_hr")]
    public bool EnableHr { get; init; }

    [JsonPropertyName("denoising_strength")]
    public float DenoisingStrength { get; init; }

    [JsonPropertyName("hr_upscaler"), JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? Upscaler { get; init; }

    [JsonPropertyName("hr_resize_x"), JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public uint? UpscaleWidth { get; init; }

    [JsonPropertyName("hr_resize_y"), JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public uint? UpscaleHeight { get; init; }


    [JsonPropertyName("override_settings")]
    public Dictionary<string, string> OverrideSettings = new();

    [JsonPropertyName("override_settings_restore_afterwards")]
    public bool RestoreAfterOverrides { get; init; }

    public TextToImageConfigRequest(TextToImageConfig config)
    {
        Prompt = config.Prompt;
        NegativePrompt = config.NegativePrompt;
        Styles = config.Styles.Select(a => a.Name).ToArray();
        Height = config.Height;
        Width = config.Width;
        Tiling = config.Tiling;
        Seed = config.Seed;
        SubSeed = config.SubSeed;
        SubSeedStrength = (float?)config.SubseedStrength;
        SeedResizeFromWidth = config.SeedResizeFromWidth;
        SeedResizeFromHeight = config.SeedResizeFromHeight;
        SamplingSteps = config.SamplingSteps;
        SamplerName = config.Sampler?.Name;
        CfgScale = (float?)config.CfgScale;
        Eta = (float?)config.Eta;
        Batches = config.Batches;
        BatcheSize = config.BatchSize;
        RestoreFaces = config.RestoreFaces;
        EnableHr = config.HighRes != null;
        if (config.HighRes != null)
        {
            DenoisingStrength = (float)config.HighRes.DenoisingStrength;
            Upscaler = config.HighRes.Upscaler.Name;
            UpscaleWidth = config.HighRes.Width;
            UpscaleHeight = config.HighRes.Height;
        }

        RestoreAfterOverrides = true;
        var model = config.Model?.Title;
        if (model != null)
            OverrideSettings.Add("sd_model_checkpoint", model);
    }
}