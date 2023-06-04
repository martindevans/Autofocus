using Autofocus.Models;
using SixLabors.ImageSharp;

namespace Autofocus.Terminal.Extensions;

public static class Base64EncodedImageExtensions
{
    public static Image ToImage(this Base64EncodedImage image)
    {
        var stream = new MemoryStream();
        stream.Write(image.Data.Span);
        stream.Position = 0;
        return Image.Load(stream);
    }

    public static Base64EncodedImage ToEncodedImage(this Image image)
    {
        var stream = new MemoryStream();
        image.SaveAsPng(stream);
        return new Base64EncodedImage(stream.ToArray());
    }
}