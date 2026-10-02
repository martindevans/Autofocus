using SkiaSharp;

namespace Autofocus.SkiaSharp.Extensions;

public static class ImageExtensions
{
    public static Base64EncodedImage ToAutofocusImage(this SKBitmap image)
    {
        using var png = image.Encode(SKEncodedImageFormat.Png, 100);
        return new Base64EncodedImage(png.AsSpan().ToArray());
    }

    public static async Task<Base64EncodedImage> ToAutofocusImageAsync(this SKBitmap image)
    {
        using var png = image.Encode(SKEncodedImageFormat.Png, 100);
        return new Base64EncodedImage(png.AsSpan().ToArray());
    }
}