using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Drawing.Processing;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;

namespace Autofocus.ImageSharp;

public static class MaskHelper
{
    /// <summary>
    /// Create a mask with one solid color in the middle, around the edge is a band that fades from that color to the inverse.
    /// </summary>
    /// <param name="width">image width</param>
    /// <param name="height">image height</param>
    /// <param name="blur">Band thickness in pixels</param>
    /// <param name="focusEdges"></param>
    /// <returns></returns>
    public static Image<Rgb24> CreateBlurOutlineMask(int width, int height, int blur, bool focusEdges = false)
    {
        var mask = new Image<Rgb24>(width, height);
        mask.Mutate(ctx =>
        {
            if (blur * 2 >= width)
                blur = width / 2;
            if (blur * 2 >= height)
                blur = height / 2;

            var rect = new RectangleF(blur, blur, width - blur * 2, height - blur * 2);

            ctx.Fill(focusEdges ? Color.White : Color.Black)
               .Fill(focusEdges ? Color.Black : Color.White, rect)
               .BoxBlur(blur);
        });

        return mask;
    }

    /// <summary>
    /// Copy the R channel from the mask into the alpha channel of the target
    /// </summary>
    /// <param name="target"></param>
    /// <param name="mask"></param>
    public static void ApplyMaskAsAlpha(Image<Rgba32> target, Image<Rgb24> mask)
    {
        Image<Rgb24>? dispose = null;
        try
        {
            if (mask.Size != target.Size)
            {
                dispose = mask.Clone();
                dispose.Mutate(x => x.Resize(target.Size));
                mask = dispose;
            }

            target.ProcessPixelRows(mask, (targetAccessor, maskAccessor) =>
            {
                for (var y = 0; y < targetAccessor.Height; y++)
                {
                    var targetRow = targetAccessor.GetRowSpan(y);
                    var maskRow = maskAccessor.GetRowSpan(y);

                    for (var x = 0; x < targetRow.Length; x++)
                        targetRow[x].A = maskRow[x].R;
                }
            });
        }
        finally
        {
            dispose?.Dispose();
        }
    }
}