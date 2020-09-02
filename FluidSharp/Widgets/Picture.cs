using FluidSharp.Layouts;
using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Text;

namespace FluidSharp.Widgets
{
    public class Picture : Widget
    {

        public SKPicture SKPicture;
        public float Opacity;

        public SKSize Size => SKPicture.CullRect.Size;

        public Picture(SKPicture skpicture, float opacity = 1f)
        {
            SKPicture = skpicture;
            Opacity = opacity;
        }

        public override SKSize Measure(MeasureCache measureCache, SKSize boundaries)
        {
            return Size;
        }

        public override SKRect PaintInternal(LayoutSurface layoutsurface, SKRect rect)
        {

            var picturesize = Size;

            float x;
            if (layoutsurface.Device.FlowDirection == SkiaSharp.TextBlocks.Enum.FlowDirection.LeftToRight)
                x = rect.Left;
            else
                x = rect.Right - picturesize.Width;

            var canvas = layoutsurface.Canvas;
            if (canvas != null)
            {
                if (Opacity == 0)
                { }
                else if (Opacity == 1)
                    canvas.DrawPicture(SKPicture, x, rect.Top);
                else
                {
                    using (var paint = new SKPaint() { Color = SKColors.Black.WithOpacity(Opacity) })
                        canvas.DrawPicture(SKPicture, x, rect.Top, paint);
                }
            }

            return new SKRect(x, rect.Top, x + picturesize.Width, rect.Top + picturesize.Height);
        }


    }
}
