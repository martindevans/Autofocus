using Autofocus.CtrlNet;

namespace Autofocus.Extensions.AfterDetailer
{
    public class Step
    {
        public required string Model { get; set; }

        public string? PositivePrompt { get; set; }
        public string? NegativePrompt { get; set; }

        public bool RestoreFaces { get; set; }

        public ControlNetModel? ControlNetModel { get; set; }
        public double? ControlNetWeight { get; set; }
        public double? ControlnetGuidanceStart { get; set; }
        public double? ControlnetGuidanceEnd { get; set; }

        internal Dictionary<string, object?> ToJsonObject()
        {
            var args = new Dictionary<string, object?>
            {
                { "ad_model", Model },

                { "ad_restore_face", RestoreFaces }

                //"ad_confidence": 0.3,
                //"ad_mask_min_ratio": 0.0,
                //"ad_mask_max_ratio": 1.0,
                //"ad_dilate_erode": 32,
                //"ad_x_offset": 0,
                //"ad_y_offset": 0,
                //"ad_mask_merge_invert": "None",
                //"ad_mask_blur": 4,
                //"ad_denoising_strength": 0.4,
                //"ad_inpaint_only_masked": true,
                //"ad_inpaint_only_masked_padding": 0,
                //"ad_use_inpaint_width_height": false,
                //"ad_inpaint_width": 512,
                //"ad_inpaint_height": 512,
                //"ad_use_steps": true,
                //"ad_steps": 28,
                //"ad_use_cfg_scale": false,
                //"ad_cfg_scale": 7.0,
            };

            if (PositivePrompt != null)
                args["ad_prompt"] = PositivePrompt;
            if (NegativePrompt != null)
                args["ad_negative_prompt"] = NegativePrompt;

            if (ControlNetModel != null)
            {
                args["ad_controlnet_model"] = ControlNetModel.Name;

                if (ControlNetWeight.HasValue)
                    args["ad_controlnet_weight"] = ControlNetWeight.Value;
                if (ControlnetGuidanceStart.HasValue)
                    args["ad_controlnet_guidance_start"] = ControlnetGuidanceStart.Value;
                if (ControlnetGuidanceEnd.HasValue)
                    args["ad_controlnet_guidance_end"] = ControlnetGuidanceEnd.Value;
            }

            return args;
        }
    }
}
