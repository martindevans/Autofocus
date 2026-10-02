using Autofocus.ImageMagick.Extensions;
using ImageMagick;
using ImageMagick.Factories;

namespace Autofocus.PixelArt;

public static class PixelArtExtensions
{
    public static async Task<IMagickImage<Q>> PixelArt<Q>(this Base64EncodedImage input, PixelArtConfig config, IMagickImageFactory<Q> factory)
        where Q : struct, IConvertible
    {
        using var inputImage = await input.ToMagickImageAsync(factory);
        return await inputImage.PixelArt(config);
    }

    public static async Task<IMagickImage<Q>> PixelArt<Q>(this IMagickImage<Q> magick, PixelArtConfig config)
        where Q : struct, IConvertible
    {
        var w = magick.Width;
        var h = magick.Height;

        // Clone and shrink, so that mutations do not change input
        magick = magick.CloneAndMutate(mut =>
        {
            mut.Resize(w / (uint)config.Strength, h / (uint)config.Strength, FilterType.Lanczos2);
        });

        // Pump up the saturation slightly
        magick.Modulate(brightness: new Percentage(100), saturation: new Percentage(110));

        // Sharpen the image to preserve edge details
        magick.AdaptiveSharpen();

        // Quantize a clone to half the number of colours
        magick.Quantize(new QuantizeSettings
        {
            Colors = (uint)Math.Max(1, config.MaxColors),
            DitherMethod = DitherMethod.Riemersma,
            ColorSpace = ColorSpace.Lab
        });
        
        // Expand back up to full size
        magick.Resize(w, h, FilterType.Point);

        return magick;
    }
}

public record PixelArtConfig
{
    /// <summary>
    /// How many colours to quantize to
    /// </summary>
    public required int MaxColors { get; init; }
    
    /// <summary>
    /// How intense the effect should be
    /// </summary>
    public required PixelArtStrength Strength { get; init; }
}

public enum PixelArtStrength
{
    Low = 2,
    Medium = 4,
    High = 6,
    VeryHigh = 8,
}