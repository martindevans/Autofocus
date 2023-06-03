using System.Text.Json.Serialization;

namespace Autofocus.Models
{
    public record ImageToImageConfig
    {
        public List<Base64EncodedImage> Images { get; } = new();

        public required string Prompt { get; init; }
        public required string NegativePrompt { get; init; }
        public List<IPromptStyle> Styles { get; init; } = new();

        public required SeedConfig Seed { get; set; }

        /*
         * {
      "resize_mode": 0,
      "denoising_strength": 0.75,
      "image_cfg_scale": 0,
      "mask": "string",
      "mask_blur": 4,
      "inpainting_fill": 0,
      "inpaint_full_res": true,
      "inpaint_full_res_padding": 0,
      "inpainting_mask_invert": 0,
      "initial_noise_multiplier": 0,
      "sampler_name": "string",
      "batch_size": 1,
      "n_iter": 1,
      "steps": 50,
      "cfg_scale": 7,
      "width": 512,
      "height": 512,
      "restore_faces": false,
      "tiling": false,
      "do_not_save_samples": false,
      "do_not_save_grid": false,
      "eta": 0,
      "s_min_uncond": 0,
      "s_churn": 0,
      "s_tmax": 0,
      "s_tmin": 0,
      "s_noise": 1,
      "override_settings": {},
      "override_settings_restore_afterwards": true,
      "script_args": [],
      "sampler_index": "Euler",
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

        public ImageToImageConfigRequest(ImageToImageConfig config)
        {
            Images = config.Images.Select(a => a.Base64()).ToArray();
            Prompt = config.Prompt;
            NegativePrompt = config.NegativePrompt;
            Styles = config.Styles.Select(a => a.Name).ToArray();
            Seed = config.Seed.Seed;
            SubSeed = config.Seed.SubSeed;
            SubSeedStrength = (float?)config.Seed.SubseedStrength;
            SeedResizeFromWidth = config.Seed.SeedResizeFromWidth;
            SeedResizeFromHeight = config.Seed.SeedResizeFromHeight;
        }
    }
}
