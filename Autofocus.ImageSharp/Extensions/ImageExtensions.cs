namespace Autofocus.ImageSharp.Extensions;

public static class ImageExtensions
{
    public static Base64EncodedImage ToAutofocusImage(this Image image)
    {
        var stream = new MemoryStream();
        image.SaveAsPng(stream);
        return new Base64EncodedImage(stream.ToArray());
    }

    public static async Task<Base64EncodedImage> ToAutofocusImageAsync(this Image image)
    {
        var stream = new MemoryStream();
        await image.SaveAsPngAsync(stream);
        return new Base64EncodedImage(stream.ToArray());
    }
}