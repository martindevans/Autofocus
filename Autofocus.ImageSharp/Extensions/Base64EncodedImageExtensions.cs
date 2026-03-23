using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;

namespace Autofocus.ImageSharp.Extensions;

public static class Base64EncodedImageExtensions
{
    public static Image ToImageSharp(this Base64EncodedImage image)
    {
        using var stream = new MemoryStream();
        stream.Write(image.Data.Span);
        stream.Position = 0;
        return Image.Load(stream);
    }

    public static async Task<Image> ToImageSharpAsync(this Base64EncodedImage image)
    {
        using var stream = new MemoryStream();
        stream.Write(image.Data.Span);
        stream.Position = 0;
        return await Image.LoadAsync(stream);
    }

    public static async Task<Image<TPixel>> ToImageSharpAsync<TPixel>(this Base64EncodedImage image)
        where TPixel : unmanaged, IPixel<TPixel>
    {
        using var stream = new MemoryStream();
        stream.Write(image.Data.Span);
        stream.Position = 0;
        return await Image.LoadAsync<TPixel>(stream);
    }
}