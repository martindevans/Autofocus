using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using Autofocus.Config;
using Autofocus.CtrlNet;
using Autofocus.Models;

namespace Autofocus;

public class StableDiffusion
{
    internal static readonly JsonSerializerOptions SerializerOptions = new()
    {
        PropertyNameCaseInsensitive = true,
        IncludeFields = true,
        Converters =
        {
            new Base64EncodedImageConverter()
        }
    };

    internal HttpClient HttpClient { get; } = new();

    public StableDiffusion(string? address = null)
        : this(address == null ? new Uri("http://127.0.0.1:7860") : new Uri(address))
    {
    }

    public StableDiffusion(Uri address)
    {
        HttpClient.BaseAddress = address;
        HttpClient.DefaultRequestHeaders.UserAgent.ParseAdd("Autofocus Agent");
    }

    public async Task<IProgress> Progress()
    {
        return (await HttpClient.GetFromJsonAsync<ProgressResponse>("/sdapi/v1/progress?skip_current_image=false", SerializerOptions))!;
    }

    #region scripts
    public async Task<IScriptsResponse> Scripts()
    {
        return (await HttpClient.GetFromJsonAsync<ScriptsResponse>("/sdapi/v1/scripts", SerializerOptions))!;
    }
    #endregion

    #region sampler
    public async Task<IEnumerable<ISampler>> Samplers()
    {
        return (await HttpClient.GetFromJsonAsync<SamplerResponse[]>("/sdapi/v1/samplers", SerializerOptions))!;
    }

    public async Task<ISampler> Sampler(string name)
    {
        var samplers = await Samplers();
        return samplers.Single(a => a.Name.Equals(name, StringComparison.InvariantCultureIgnoreCase));
    }
    #endregion

    #region upscaler
    public async Task<IEnumerable<IUpscaler>> Upscalers()
    {
        var upscalers = await HttpClient.GetFromJsonAsync<UpscalerResponse[]>("/sdapi/v1/upscalers", SerializerOptions);

        for (var i = 0; i < upscalers!.Length; i++)
            upscalers[i].Index = i;

        return upscalers;
    }

    public async Task<IUpscaler> Upscaler(string name)
    {
        var models = await Upscalers();
        return models.Single(a => a.Name.Equals(name, StringComparison.InvariantCultureIgnoreCase));
    }
    #endregion

    #region style
    public async Task<IEnumerable<IPromptStyle>> Styles()
    {
        return (await HttpClient.GetFromJsonAsync<PromptStyleResponse[]>("/sdapi/v1/prompt-styles", SerializerOptions))!;
    }

    public async Task<IPromptStyle> Style(string name)
    {
        var styles = await Styles();
        return styles.Single(a => a.Name.Equals(name, StringComparison.InvariantCultureIgnoreCase));
    }
    #endregion

    #region SD models
    public async Task<IEnumerable<IStableDiffusionModel>> StableDiffusionModels()
    {
        return (await HttpClient.GetFromJsonAsync<StableDiffusionModelResponse[]>("/sdapi/v1/sd-models", SerializerOptions))!;
    }

    public async Task<IStableDiffusionModel> StableDiffusionModel(string name)
    {
        var models = await StableDiffusionModels();
        return models.Single(a => a.ModelName.Equals(name, StringComparison.InvariantCultureIgnoreCase));
    }
    #endregion

    #region embeddings
    public async Task<IEmbeddings> Embeddings()
    {
        return (await HttpClient.GetFromJsonAsync<EmbeddingsResponse>("/sdapi/v1/embeddings", SerializerOptions))!;
    }
    #endregion

    #region memory
    public async Task<IMemory> Memory()
    {
        return (await HttpClient.GetFromJsonAsync<MemoryResponse>("/sdapi/v1/memory", SerializerOptions))!;
    }
    #endregion

    #region PNG Info
    public async Task<IPngInfo> PngInfo(Base64EncodedImage image)
    {
        var request = new PngInfoRequest(image);

        var response = await HttpClient.PostAsJsonAsync("/sdapi/v1/png-info", request, SerializerOptions);

        var result = await response
            .EnsureSuccessStatusCode()
            .Content
            .ReadFromJsonAsync<PngInfoResponse>();

        return result!;
    }
    #endregion

    #region Text2Image
    public async Task<ITextToImageResult> TextToImage(TextToImageConfig config)
    {
        var request = new TextToImageConfigRequest(config);

        var response = await HttpClient.PostAsJsonAsync("/sdapi/v1/txt2img", request, SerializerOptions);

        var result = await response
                          .EnsureSuccessStatusCode()
                          .Content
                          .ReadFromJsonAsync<TextToImageResultResponse>(SerializerOptions);

        return result!;
    }
    #endregion

    #region Image2Image
    public async Task<IImageToImageResult> Image2Image(ImageToImageConfig config)
    {
        var request = new ImageToImageConfigRequest(config);

        var response = await HttpClient.PostAsJsonAsync("/sdapi/v1/img2img", request, SerializerOptions);

        var result = await response
                          .EnsureSuccessStatusCode()
                          .Content
                          .ReadFromJsonAsync<ImageToImageResultResponse>(SerializerOptions);

        return result!;
    }
    #endregion

    #region interrogate
    public Task<IInterrogateResult> Interrogate(Base64EncodedImage image, InterrogateModel model = InterrogateModel.CLIP)
    {
        return Interrogate(new InterrogateConfig
        {
            Image = image,
            Model = model
        });
    }

    public async Task<IInterrogateResult> Interrogate(InterrogateConfig config)
    {
        var request = new InterrogateConfigRequest(config);

        var response = await HttpClient.PostAsJsonAsync("/sdapi/v1/interrogate", request, SerializerOptions);

        var result = await response
                          .EnsureSuccessStatusCode()
                          .Content
                          .ReadFromJsonAsync<InterrogateResultResponse>(SerializerOptions);

        return result!;
    }
    #endregion

    #region controlnet
    public async Task<ControlNet?> TryGetControlNet()
    {
        // Get version of controlnet
        var versionResponse = await HttpClient.GetAsync("/controlnet/version");

        // Check if exists at all!
        if (versionResponse.StatusCode == HttpStatusCode.NotFound)
            return null;

        // Check that it is v2
        var version = await JsonSerializer.DeserializeAsync<ControlNetVersionResponse>(
            await versionResponse.EnsureSuccessStatusCode().Content.ReadAsStreamAsync()
        );
        if (version is not { Version: 2 })
            throw new InvalidOperationException($"Unknown controlnet version: '{version?.Version}'");

        return new ControlNet(this);
    }
    #endregion
}