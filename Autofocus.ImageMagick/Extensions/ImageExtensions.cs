using ImageMagick;

namespace Autofocus.ImageMagick.Extensions;

public static class ImageExtensions
{
    public static Base64EncodedImage ToAutofocusImage(this IMagickImage image)
    {
        var fmt = image.Format;
        var qal = image.Quality;
        try
        {
            image.Format = MagickFormat.Png;
            image.Quality = 100;
            
            using var mem = new MemoryStream();
            image.Write(mem);

            return new Base64EncodedImage(mem.ToArray());
        }
        finally
        {
            image.Format = fmt;
            image.Quality = qal;
        }
    }

    public static async Task<Base64EncodedImage> ToAutofocusImageAsync(this IMagickImage image)
    {
        var fmt = image.Format;
        var qal = image.Quality;
        try
        {
            image.Format = MagickFormat.Png;
            image.Quality = 100;

            using var mem = new MemoryStream();
            await image.WriteAsync(mem);

            return new Base64EncodedImage(mem.ToArray());
        }
        finally
        {
            image.Format = fmt;
            image.Quality = qal;
        }
    }
}