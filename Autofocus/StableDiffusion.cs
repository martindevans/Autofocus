using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using Autofocus.Models;

namespace Autofocus;

public class StableDiffusion
{
    private readonly HttpClient _httpClient;
    private readonly JsonSerializerOptions _serializerOptions;

    public StableDiffusion(Uri? address = null)
    {
        _httpClient = new HttpClient();
        _httpClient.BaseAddress = address ?? new Uri("http://127.0.0.1:7860");
        _httpClient.DefaultRequestHeaders.UserAgent.ParseAdd("Autofocus Agent");

        _serializerOptions = new JsonSerializerOptions()
        {
            PropertyNameCaseInsensitive = true,
            IncludeFields = true,
            Converters =
            {
                new Base64EncodedImageConverter()
            }
        };
    }

    public async Task<IProgress> Progress()
    {
        return (await _httpClient.GetFromJsonAsync<ProgressResponse>("/sdapi/v1/progress?skip_current_image=false", _serializerOptions))!;
    }


    public async Task<IEnumerable<ISampler>> Samplers()
    {
        return (await _httpClient.GetFromJsonAsync<SamplerResponse[]>("/sdapi/v1/samplers", _serializerOptions))!;
    }

    public async Task<ISampler?> Sampler(string name)
    {
        var samplers = await Samplers();
        return samplers.FirstOrDefault(a => a.Name.Equals(name, StringComparison.InvariantCultureIgnoreCase));
    }


    public async Task<IEnumerable<IUpscaler>> Upscalers()
    {
        return (await _httpClient.GetFromJsonAsync<UpscalerResponse[]>("/sdapi/v1/upscalers", _serializerOptions))!;
    }


    public async Task<IEnumerable<IStableDiffusionModel>> StableDiffusionModels()
    {
        return (await _httpClient.GetFromJsonAsync<StableDiffusionModelResponse[]>("/sdapi/v1/sd-models", _serializerOptions))!;
    }

    public async Task<IStableDiffusionModel?> StableDiffusionModel(string name)
    {
        var models = await StableDiffusionModels();
        return models.FirstOrDefault(a => a.ModelName.Equals(name, StringComparison.InvariantCultureIgnoreCase));
    }


    public async Task<IEmbeddings> Embeddings()
    {
        return (await _httpClient.GetFromJsonAsync<EmbeddingsResponse>("/sdapi/v1/embeddings", _serializerOptions))!;
    }


    public async Task<IMemory> Memory()
    {
        return (await _httpClient.GetFromJsonAsync<MemoryResponse>("/sdapi/v1/memory", _serializerOptions))!;
    }


    public async Task<IPngInfo> PngInfo(Base64EncodedImage image)
    {
        var request = new PngInfoRequest(image);

        var response = await _httpClient.PostAsJsonAsync("/sdapi/v1/png-info", request, _serializerOptions);

        var result = await response
            .EnsureSuccessStatusCode()
            .Content
            .ReadFromJsonAsync<PngInfoResponse>();

        return result!;
    }


    public async Task<ITextToImageResult> TextToImage(TextToImageConfig config)
    {
        var request = new TextToImageConfigRequest(config);

        var response = await _httpClient.PostAsJsonAsync("/sdapi/v1/txt2img", request, _serializerOptions);

        var result = await response
                          .EnsureSuccessStatusCode()
                          .Content
                          .ReadFromJsonAsync<TextToImageResultResponse>(_serializerOptions);

        return result!;
    }
}