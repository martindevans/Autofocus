using SkiaSharp;

namespace Autofocus.SkiaSharp.Extensions;

public static class Base64EncodedImageExtensions
{
    public static SKImage ToSkiaSharpImage(this Base64EncodedImage image)
    {
        return SKImage.FromEncodedData(image.Data.Span);
    }

    public static async Task<SKImage> ToSkiaSharpImageAsync(this Base64EncodedImage image)
    {
        return await Task.Run(() => SKImage.FromEncodedData(image.Data.Span));
    }

    public static SKBitmap ToSkiaSharpBitmap(this Base64EncodedImage image)
    {
        return SKBitmap.Decode(image.Data.Span);
    }

    public static async Task<SKBitmap> ToSkiaSharpBitmapAsync(this Base64EncodedImage image)
    {
        return await Task.Run(() => SKBitmap.Decode(image.Data.Span));
    }
}