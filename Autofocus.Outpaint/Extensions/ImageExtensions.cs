using System.Numerics;
using System.Security.Cryptography;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;

namespace Autofocus.Outpaint.Extensions
{
    internal static class ImageExtensions
    {
        public static void Bleed(this Image<Rgba32> image, Rectangle from, int radius, int? seed)
        {
            var rng = new Random(seed ?? RandomNumberGenerator.GetInt32(int.MaxValue));

            for (var i = 0; i < image.Width; i++)
            {
                for (var j = 0; j < image.Height; j++)
                {
                    if (from.Contains(i, j))
                        continue;

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
                        X = Math.Clamp(i + (int)Math.Round((rng.NextSingle() * 2 - 1) * distance), from.Left, from.Right),
                        Y = Math.Clamp(j + (int)Math.Round((rng.NextSingle() * 2 - 1) * distance), from.Top, from.Bottom)
                    };
                    var closestPixel = image[offsetClosest.X, offsetClosest.Y].ToVector4();

                    // Randomize colour slightly (based on distance)
                    var distanceFactor = distance / radius;
                    closestPixel += new Vector4(
                        (rng.NextSingle() - 0.5f) * distanceFactor,
                        (rng.NextSingle() - 0.5f) * distanceFactor,
                        (rng.NextSingle() - 0.5f) * distanceFactor,
                        0
                    );

                    image[i, j] = new Rgba32(closestPixel);
                }
            }
        }
    }
}
