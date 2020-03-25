using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Text;

namespace FluidSharp.Paint
{
    public class ImagePainter
    {

        public Device Device;

        private SKPaint ImagePaint = new SKPaint() { FilterQuality = SKFilterQuality.High, Color = SKColors.White };

        public ImagePainter(Device device)
        {
            Device = device;
        }

        public void DrawImage(SKCanvas canvas, string name, SKRect rect)
        {

            if (canvas == null)
                return;

            if (Device.ImageSource == null)
                throw new NullReferenceException("cannot draw an image by name when ImagePainter.ImageSource is null");

            var image = Device.ImageSource.GetImage(name);
            if (image == null)
                throw new NullReferenceException($"image not found: {name}");

            var paintrect = rect;
            if (Device.PixelRounding)
            {
                var l = (float)Math.Round(rect.Left);
                var t = (float)Math.Round(rect.Top);
                var w = (float)Math.Round(rect.Width);
                var h = (float)Math.Round(rect.Height);
                paintrect = new SKRect(l, t, l + w, t + h);
            }

            canvas.DrawImage(image, paintrect, ImagePaint);

        }

    }
}
