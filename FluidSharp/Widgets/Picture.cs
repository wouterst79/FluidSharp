using FluidSharp.Layouts;
using FluidSharp.Paint;
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
        private float opacity;
        public float Opacity
        {
            get => opacity; set
            {
#if DEBUG
                if (!OwnsOpacity) throw new Exception("can't set opacity on default pictures");
#endif
                opacity = value;
            }
        }
        public bool AutoFlipRTL;

        public SKSize Size;

        private bool OwnsOpacity;

        public Picture(SKImage image, bool autoFlipRtl = true, float opacity = 1f)
        {
            SKImage = image;
            AutoFlipRTL = autoFlipRtl;
            this.opacity = opacity;
            Size = new SKSize(image.Width / ScreenScale, image.Height / ScreenScale);
        }

        public Picture WithOpacity(float opacity)
        {
#if DEBUG
            if (OwnsOpacity) throw new Exception("this widget already owns opacity");
#endif
            return new Picture(SKImage, AutoFlipRTL, opacity) { OwnsOpacity = true };
        }

        public override SKSize Measure(MeasureCache measureCache, SKSize boundaries)
        {
            return Size;
        }

        public override SKRect PaintInternal(LayoutSurface layoutsurface, SKRect rect)
        {
            var drawrect = rect.HorizontalAlign(Size, HorizontalAlignment.Near, layoutsurface.FlowDirection);
            Paint(layoutsurface.Canvas, drawrect, layoutsurface.IsRtl);
            return rect.WithHeight(Size.Height);
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

                canvas.DrawImage(SKImage, rect, PaintCache.ImageSamplingOptions, PaintCache.GetImagePaint(Opacity));

                if (flip)
                    canvas.Restore();

            }

        }

    }
}
