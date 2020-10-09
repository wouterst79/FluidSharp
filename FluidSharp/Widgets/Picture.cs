using FluidSharp.Layouts;
using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Text;

namespace FluidSharp.Widgets
{
    public class Picture : Widget
    {

        public static float ScreenScale;

        public SKImage SKImage;
        public float Opacity;
        public bool AutoFlipRTL;

        public SKSize Size;

        public Picture(SKImage image, bool autoFlipRtl = true, float opacity = 1f)
        {
            SKImage = image;
            AutoFlipRTL = autoFlipRtl;
            Opacity = opacity;
            Size = new SKSize(image.Width / ScreenScale, image.Height / ScreenScale);
        }

        public override SKSize Measure(MeasureCache measureCache, SKSize boundaries)
        {
            return Size;
        }

        public override SKRect PaintInternal(LayoutSurface layoutsurface, SKRect rect)
        {
            Paint(layoutsurface.Canvas, rect, layoutsurface.IsRtl);
            return rect;
        }

        public void Paint(SKCanvas canvas, SKRect rect, bool isrtl)
        {

            if (canvas != null && Opacity > 0)
            {

                var flip = AutoFlipRTL && isrtl;
                if (flip)
                {
                    var matrix = SKMatrix.CreateScale(-1, 1);
                    canvas.Save();
                    canvas.Concat(ref matrix);
                    rect = new SKRect(-rect.Right, rect.Top, -rect.Left, rect.Bottom);
                }

                using (var paint = new SKPaint() { Color = SKColors.Black.WithOpacity(Opacity), FilterQuality = SKFilterQuality.High })
                    canvas.DrawImage(SKImage, rect, paint);

                if (flip)
                    canvas.Restore();

            }

        }

    }
}
