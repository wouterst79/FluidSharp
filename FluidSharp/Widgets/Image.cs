using FluidSharp.Layouts;
using FluidSharp.Paint;
using SkiaSharp;
using SkiaSharp.TextBlocks.Enum;
using System;
using System.Collections.Generic;
using System.Text;

namespace FluidSharp.Widgets
{
    public class Image : Widget
    {

        private readonly string Name;
        private readonly int Width;
        private readonly int Height;

        public Image(string name, int width, int height)
        {
            Name = name;
            Width = width;
            Height = height;
        }

        public override SKSize Measure(MeasureCache measureCache, SKSize boundaries)
        {
            return new SKSize(Width, Height);
        }

        public override SKRect PaintInternal(LayoutSurface layoutsurface, SKRect rect)
        {
            var x = rect.Left;
            if (layoutsurface.Device.FlowDirection == FlowDirection.RightToLeft)
                x = rect.Right - Width;

            var y = rect.Top;
            var dest = new SKRect(x, y, x + Width, y + Height);

            layoutsurface.Device.ImagePainter.DrawImage(layoutsurface.Canvas, Name, dest);

            return dest;
        }
    }
}
