using FluidSharp.Layouts;
using FluidSharp.Paint;
using SkiaSharp;
using SkiaSharp.TextBlocks.Enum;
using System;
using System.Collections.Generic;
using System.Text;

namespace FluidSharp.Widgets
{

    public class Circle : Widget
    {


        public float Size;
        public bool Fill;

        public SKColor BackgroundColor;
        public SKColor BorderColor;

        public Func<SKImageFilter> ImageFilter;

        public Widget ClippedContents;

        private const bool Antialias = true;

        public Circle(float size, SKColor backgroundcolor, SKColor bordercolor, Widget clippedContents = null)
        {
            Size = size;
            BackgroundColor = backgroundcolor;
            BorderColor = bordercolor;
            ClippedContents = clippedContents;
        }

        public override SKSize Measure(MeasureCache measureCache, SKSize boundaries)
        {
            if (Size > 0)
                return new SKSize(Size, Size);
            var min = Math.Min(boundaries.Width, boundaries.Height);
            return new SKSize(min, min);
        }

        public override SKRect PaintInternal(LayoutSurface layoutsurface, SKRect rect)
        {

            var size = Measure(null, rect.Size);

            // tight is default;
            var drawrect = new SKRect(0, rect.Top, 0, rect.Top + size.Height);
            if (layoutsurface.Device.FlowDirection == FlowDirection.LeftToRight)
            {
                drawrect.Left = rect.Left;
                drawrect.Right = rect.Left + size.Width;
            }
            else
            {
                drawrect.Left = rect.Right - size.Width;
                drawrect.Right = rect.Right;
            }


            var point = new SKPoint(drawrect.MidX, drawrect.MidY);
            var radius = size.Width / 2;

            if (layoutsurface.Canvas != null)
            {

                if (BackgroundColor != null && BackgroundColor.Alpha != 0)
                {
                    var paint = PaintCache.GetBackgroundPaint(BackgroundColor, Antialias, ImageFilter);
                    layoutsurface.Canvas.DrawCircle(point, radius, paint);
                }

                if (BorderColor != null && BorderColor.Alpha != 0)
                {
                    layoutsurface.Canvas.DrawCircle(point, radius, PaintCache.GetBorderPaint(BorderColor, Antialias, 1));
                }

            }

            if (ClippedContents != null)
            {

                var clippath = new SKPath();
                clippath.AddCircle(point.X, point.Y, radius);
                layoutsurface.ClipPath(clippath);

                layoutsurface.Paint(ClippedContents, drawrect);

                layoutsurface.ResetPathClip();

            }

            return drawrect;
        }

    }

}
