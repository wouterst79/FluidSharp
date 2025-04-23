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

        public ImageSource? Source;
        public float Width;
        public float Height;
        public ScaleMode ScaleMode;
        public float Opacity = 1;

        public Image(ImageSource? source, float width, float height, ScaleMode scaleMode = ScaleMode.Strech)
        {
            Source = source;
            Width = width;
            Height = height;
            ScaleMode = scaleMode;
        }

        public override SKSize Measure(MeasureCache measureCache, SKSize boundaries)
        {
            if (ScaleMode == ScaleMode.Strech)
                return new SKSize(Width, Height);

            var w = boundaries.Width == 0 ? Width : Math.Min(boundaries.Width, Width);
            var h = boundaries.Height == 0 ? Height : Math.Min(boundaries.Height, Height);

            return new SKSize(w, h);
            //                return boundaries;
        }

        public override SKRect PaintInternal(LayoutSurface layoutsurface, SKRect rect)
        {

            var x = rect.Left;
            if (layoutsurface.Device.FlowDirection == FlowDirection.RightToLeft)
                x = rect.Right - Width;

            var y = rect.Top;
            var dest = ScaleMode == ScaleMode.Strech ? new SKRect(x, y, x + Width, y + Height) : rect;

            if (layoutsurface.Canvas == null || Source == null || Source.Name == null)
                return dest;

            var image = layoutsurface.MeasureCache.ImageCache.GetImage(Source);
            if (image == null)
                return dest;

            var paintrect = dest.Scale(ScaleMode, new SKSize(image.Width, image.Height));

            if (layoutsurface.Device.PixelRounding)
            {
                var l = (float)Math.Round(paintrect.Left);
                var t = (float)Math.Round(paintrect.Top);
                var w = (float)Math.Round(paintrect.Width);
                var h = (float)Math.Round(paintrect.Height);
                paintrect = new SKRect(l, t, l + w, t + h);
            }

            var opacity = Opacity;
            if (opacity <= 0)
            {

            }
            else
            {
                layoutsurface.Canvas.DrawImage(image, paintrect, PaintCache.ImageSamplingOptions, PaintCache.GetImagePaint(opacity));
            }

            return dest;
        }
    }
}
