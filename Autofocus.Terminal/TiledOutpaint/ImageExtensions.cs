using SixLabors.ImageSharp.Processing.Processors.Quantization;
using System.Numerics;
using System.Security.Cryptography;

namespace Autofocus.Terminal.TiledOutpaint;

public static class ImageExtensions
{
    public static void Bleed(this Image<Rgba32> image, Rectangle from, int radius, int? seed, float distanceRandFactor)
    {
        var rng = new Random(seed ?? RandomNumberGenerator.GetInt32(int.MaxValue));

        for (var i = 0; i < image.Width; i++)
        {
            for (var j = 0; j < image.Height; j++)
            {
                if (from.Contains(i, j))
                    continue;

                var pixel = image[i, j].ToVector4();

                // Get the distance to the closest point on the rectangle
                var actualClosest = new Point
                {
                    X = Math.Clamp(i, from.Left, from.Right),
                    Y = Math.Clamp(j, from.Top, from.Bottom)
                };
                var distance = Vector2.Distance(new Vector2(i, j), new Vector2(actualClosest.X, actualClosest.Y));

                // Set another point (still on the rectangle border) randomly offset, more randomness with distance
                var offsetClosest = new Point
                {
                    X = Math.Clamp(i + (int)Math.Round((rng.NextSingle() * 2 - 1) * distance * distanceRandFactor), from.Left, from.Right),
                    Y = Math.Clamp(j + (int)Math.Round((rng.NextSingle() * 2 - 1) * distance * distanceRandFactor), from.Top, from.Bottom)
                };
                var closestPixel = image[offsetClosest.X, offsetClosest.Y].ToVector4();

                if (distance < radius)
                    image[i, j] = new Rgba32(Vector4.Lerp(closestPixel, pixel, MathF.Pow(distance / radius, 1)));
            }
        }
    }

    public static Rgba32 AverageColor(this Image image)
    {
        using var averageImg = image.CloneAs<Rgba32>();
        averageImg.Mutate(ctx => ctx.Quantize(new OctreeQuantizer(new QuantizerOptions
        {
            Dither = null,
            MaxColors = 1,
        })));
        return averageImg[0, 0];
    }
}