using SixLabors.ImageSharp;

namespace Autofocus.FeatureRepaint.Extensions;

internal static class RectangleExtensions
{
    public static float AspectRatio(this Rectangle rect)
    {
        return (float)rect.Width / rect.Height;
    }
}