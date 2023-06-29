namespace Autofocus.ImageSharp.Extensions;

public static class Base64EncodedImageExtensions
{
    public static Image ToImageSharp(this Base64EncodedImage image)
    {
        var stream = new MemoryStream();
        stream.Write(image.Data.Span);
        stream.Position = 0;
        return Image.Load(stream);
    }

    public static Task<Image> ToImageSharpAsync(this Base64EncodedImage image)
    {
        var stream = new MemoryStream();
        stream.Write(image.Data.Span);
        stream.Position = 0;
        return Image.LoadAsync(stream);
    }

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