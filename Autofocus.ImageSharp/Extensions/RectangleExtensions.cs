using SixLabors.ImageSharp;

namespace Autofocus.ImageSharp.Extensions
{
    public static class RectangleExtensions
    {
        public static float AspectRatio(this Rectangle rect)
        {
            return (float)rect.Width / rect.Height;
        }

        public static System.Drawing.Rectangle ToSystemDrawing(this SixLabors.ImageSharp.Rectangle rect)
        {
            return new System.Drawing.Rectangle(rect.X, rect.Y, rect.Width, rect.Height);
        }

        public static Rectangle ToImageSharp(this System.Drawing.Rectangle rect)
        {
            return new Rectangle(rect.X, rect.Y, rect.Width, rect.Height);
        }
    }
}
