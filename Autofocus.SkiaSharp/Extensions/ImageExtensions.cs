using SkiaSharp;

namespace Autofocus.SkiaSharp.Extensions;

public static class ImageExtensions
{
    public static Base64EncodedImage ToAutofocusImage(this SKImage image)
    {
        using var png = image.Encode(SKEncodedImageFormat.Png, 100);
        return new Base64EncodedImage(png.AsSpan().ToArray());
    }

    public static async Task<Base64EncodedImage> ToAutofocusImageAsync(this SKImage image)
    {
        using var png = await Task.Run(() => image.Encode(SKEncodedImageFormat.Png, 100));
        return new Base64EncodedImage(png.AsSpan().ToArray());
    }
}