using System.Net.Http.Json;
using System.Text.Json;
using Autofocus.CtrlNet;

namespace Autofocus;

public class ControlNet
{
    internal static readonly JsonSerializerOptions SerializerOptions = StableDiffusion.SerializerOptions;

    private readonly StableDiffusion _api;

    internal ControlNet(StableDiffusion api)
    {
        _api = api;
    }

    #region models
    public async Task<IEnumerable<ControlNetModel>> Models()
    {
        var response = await _api.FastHttpClient.GetFromJsonAsync<ControlNetModelListResponse>("controlnet/model_list", SerializerOptions);
        return response!.ModelList.Select(a => new ControlNetModel(a));
    }

    public async Task<ControlNetModel> Model(string name)
    {
        bool IsMatch(ControlNetModel a)
        {
            if (a.Name.Equals(name, StringComparison.InvariantCultureIgnoreCase))
                return true;

            var idx = a.Name.IndexOf(" [", StringComparison.InvariantCultureIgnoreCase);
            if (idx < 0)
                return false;

            var cleaned = a.Name[..idx];
            return cleaned.Equals(name, StringComparison.InvariantCultureIgnoreCase);
        }

        var models = await Models();
        return models.Single(IsMatch);
    }
    #endregion

    #region modules
    public async Task<IEnumerable<ControlNetModule>> Modules()
    {
        var response = await _api.FastHttpClient.GetFromJsonAsync<ControlNetModuleListResponse>("controlnet/module_list", SerializerOptions);

        return response!.Details.Select(a =>
            new ControlNetModule(
                a.Key,
                Parameter(a.Value.Sliders.FirstOrDefault(), true),
                a.Value.Sliders.Skip(1).Select(x => Parameter(x)!).ToArray()
            )
        );

        static ControlNetModule.Parameter? Parameter(ControlnetModuleDetailSliderResponse? slider, bool allownull = false)
        {
            if (slider == null)
            {
                if (allownull)
                    return null;
                throw new ArgumentNullException(nameof(slider));
            }

            return new ControlNetModule.Parameter(slider.Name, slider.Value, slider.Min, slider.Max);
        }
    }

    public async Task<ControlNetModule> Module(string name)
    {
        var models = await Modules();
        return models.Single(a => a.Name.Equals(name, StringComparison.InvariantCultureIgnoreCase));
    }
    #endregion

    public async Task<IControlNetPreprocess> Preprocess(ControlNetPreprocessConfig config)
    {
        var request = await _api.FastHttpClient.PostAsJsonAsync("controlnet/detect", new ControlNetPreprocessConfigModel(config), SerializerOptions);

        var response = await request
            .EnsureSuccessStatusCode()
            .Content
            .ReadFromJsonAsync<ControlNetPreprocessResponse>(SerializerOptions);

        return response!;
    }
}