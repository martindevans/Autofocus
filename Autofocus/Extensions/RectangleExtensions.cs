using System.Drawing;

namespace Autofocus.Extensions;

public static class RectangleExtensions
{
    public static float AspectRatio(this Rectangle rect)
    {
        return (float)rect.Width / rect.Height;
    }
}