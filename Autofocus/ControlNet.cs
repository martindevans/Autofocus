using System.Net.Http.Json;
using System.Text.Json;
using Autofocus.Config;
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
        var response = await _api.HttpClient.GetFromJsonAsync<ControlNetModelListResponse>("controlnet/model_list", SerializerOptions);
        return response!.ModelList.Select(a => new ControlNetModel(a));
    }

    public async Task<ControlNetModel> Model(string name)
    {
        var models = await Models();
        return models.Single(a => a.Name.Equals(name, StringComparison.InvariantCultureIgnoreCase));
    }
    #endregion

    #region modules
    public async Task<IEnumerable<ControlNetModule>> Modules()
    {
        var response = await _api.HttpClient.GetFromJsonAsync<ControlNetModuleListResponse>("controlnet/module_list", SerializerOptions);

        return response!.ModuleList.Select(a => new ControlNetModule(a));
    }

    public async Task<ControlNetModule> Module(string name)
    {
        var models = await Modules();
        return models.Single(a => a.Name.Equals(name, StringComparison.InvariantCultureIgnoreCase));
    }
    #endregion

    public async Task<IControlNetPreprocess> Preprocess(ControlNetPreprocessConfig config)
    {
        var request = await _api.HttpClient.PostAsJsonAsync("controlnet/detect", new ControlNetPreprocessConfigRequest(config), SerializerOptions);

        var response = await request
            .EnsureSuccessStatusCode()
            .Content
            .ReadFromJsonAsync<ControlNetPreprocessResponse>(SerializerOptions);

        return response!;
    }
}