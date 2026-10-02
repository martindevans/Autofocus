using System.Drawing;

namespace Autofocus.ImageSharp.Extensions;

public static class PointFExtensions
{
    public static PointF ToSystemDrawing(this SixLabors.ImageSharp.PointF pf)
    {
        return new PointF(pf.X, pf.Y);
    }

    public static SixLabors.ImageSharp.PointF ToImageSharp(this PointF pf)
    {
        return new SixLabors.ImageSharp.PointF(pf.X, pf.Y);
    }
}