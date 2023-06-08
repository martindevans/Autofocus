using System.Text.Json.Serialization;
using Autofocus.Models;

namespace Autofocus.Config;

public record ImageToImageConfig
{
    public List<Base64EncodedImage> Images { get; } = new();

    public required PromptConfig Prompt { get; set; }
    public required SeedConfig Seed { get; set; }
    public required SamplerConfig Sampler { get; set; }
    public required IStableDiffusionModel Model { get; set; }

    public double? DenoisingStrength { get; set; }
    public uint? Width { get; set; }
    public uint? Height { get; set; }
    public bool Tiling { get; set; }

    public int? BatchSize { get; set; }
    public int? Batches { get; set; }

    public bool RestoreFaces { get; set; }

    /*
     * {
  "resize_mode": 0,
  "image_cfg_scale": 0,
  "mask": "string",
  "mask_blur": 4,
  "inpainting_fill": 0,
  "inpaint_full_res": true,
  "inpaint_full_res_padding": 0,
  "inpainting_mask_invert": 0,
  "initial_noise_multiplier": 0,
  "do_not_save_samples": false,
  "do_not_save_grid": false,
  "s_min_uncond": 0,
  "s_churn": 0,
  "s_tmax": 0,
  "s_tmin": 0,
  "s_noise": 1,
  "script_args": [],
  "include_init_images": false,
  "script_name": "string",
  "send_images": true,
  "save_images": false,
  "alwayson_scripts": {}
}
     */
}

internal class ImageToImageConfigRequest
{
    [JsonPropertyName("init_images")]
    public string[] Images { get; init; }

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

    [JsonPropertyName("n_iter"), JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public int? Batches { get; init; }

    [JsonPropertyName("batch_size"), JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public int? BatcheSize { get; init; }

    [JsonPropertyName("restore_faces")]
    public bool RestoreFaces { get; init; }

    [JsonPropertyName("steps"), JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public int? SamplingSteps { get; init; }

    [JsonPropertyName("sampler_name"), JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? SamplerName { get; init; }

    [JsonPropertyName("cfg_scale"), JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public float? CfgScale { get; init; }

    [JsonPropertyName("eta"), JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public float? Eta { get; init; }

    [JsonPropertyName("denoising_strength"), JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public float? DenoisingStrength { get; init; }

    [JsonPropertyName("override_settings")]
    public Dictionary<string, string> OverrideSettings = new();

    [JsonPropertyName("override_settings_restore_afterwards")]
    public bool RestoreAfterOverrides { get; init; }

    public ImageToImageConfigRequest(ImageToImageConfig config)
    {
        Images = config.Images.Select(a => a.Base64()).ToArray();
        Prompt = config.Prompt.Negative;
        NegativePrompt = config.Prompt.Negative;
        Styles = config.Prompt.Styles.Select(a => a.Name).ToArray();
        Seed = config.Seed.Seed;
        SubSeed = config.Seed.SubSeed;
        SubSeedStrength = (float?)config.Seed.SubseedStrength;
        SeedResizeFromWidth = config.Seed.SeedResizeFromWidth;
        SeedResizeFromHeight = config.Seed.SeedResizeFromHeight;
        Width = config.Width;
        Height = config.Height;
        Tiling = config.Tiling;
        BatcheSize = config.BatchSize;
        Batches = config.Batches;
        RestoreFaces = config.RestoreFaces;
        SamplingSteps = config.Sampler.SamplingSteps;
        CfgScale = (float?)config.Sampler.CfgScale;
        SamplerName = config.Sampler.Sampler.Name;
        Eta = (float?)config.Sampler.Eta;
        DenoisingStrength = (float?)config.DenoisingStrength;

        RestoreAfterOverrides = true;
        OverrideSettings.Add("sd_model_checkpoint", config.Model.Title);
    }
}