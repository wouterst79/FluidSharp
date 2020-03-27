using FluidSharp.Layouts;
using FluidSharp.Paint;
using FluidSharp.Paint.Images;
using SkiaSharp;
using SkiaSharp.TextBlocks.Enum;
using System;
using System.Collections.Generic;
using System.Text;

namespace FluidSharp.Widgets
{
    public class Image : Widget
    {

        private readonly ImageSource Source;
        private readonly int Width;
        private readonly int Height;

        public Image(ImageSource source, int width, int height)
        {
            Source = source;
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

            if (layoutsurface.Canvas == null || Source == null || Source.Name == null)
                return dest;

            var image = layoutsurface.MeasureCache.ImageCache.GetImage(Source);
            if (image == null)
                return dest;

            var paintrect = rect;
            if (layoutsurface.Device.PixelRounding)
            {
                var l = (float)Math.Round(rect.Left);
                var t = (float)Math.Round(rect.Top);
                var w = (float)Math.Round(rect.Width);
                var h = (float)Math.Round(rect.Height);
                paintrect = new SKRect(l, t, l + w, t + h);
            }

            using (var paint = new SKPaint() { FilterQuality = SKFilterQuality.High })
                layoutsurface.Canvas.DrawImage(image, paintrect, paint);

            return dest;
        }
    }
}
