using ImageMagick;
using ImageMagick.Factories;

namespace Autofocus.ImageMagick.Extensions;

public static class Base64EncodedImageExtensions
{
    public static IMagickImage<Q> ToMagickImage<Q>(this Base64EncodedImage image, IMagickImageFactory<Q> factory)
        where Q : struct, IConvertible
    {
        return factory.Create(image.Data.Span);
    }

    public static Task<IMagickImage<Q>> ToMagickImageAsync<Q>(this Base64EncodedImage image, IMagickImageFactory<Q> factory)
        where Q : struct, IConvertible
    {
        var mem = new MemoryStream(image.Data.ToArray());
        return factory.CreateAsync(mem);
    }
}