using System.Text.Json.Serialization;

namespace Autofocus.Models;

public interface IImageToImageResult
{
    public IReadOnlyList<Base64EncodedImage> Images { get; }
    //todo:public IImageToImageResultParameters Parameters { get; }
}

internal class ImageToImageResultResponse
    : IImageToImageResult
{
    [JsonPropertyName("images")] public Base64EncodedImage[] Images { get; init; } = null!;
    [JsonPropertyName("parameters")] public TextToImageResultParametersResponse Parameters { get; init; } = null!;

    IReadOnlyList<Base64EncodedImage> IImageToImageResult.Images => Images;
    //todo:ITextToImageResultParameters IImageToImageResult.Parameters => Parameters;
}