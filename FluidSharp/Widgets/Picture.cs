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
        public bool AutoFlipRTL;

        public SKSize Size => SKPicture.CullRect.Size;

        public Picture(SKPicture skpicture, bool autoFlipRtl = true, float opacity = 1f)
        {
            SKPicture = skpicture;
            AutoFlipRTL = autoFlipRtl;
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
            var y = rect.Top;

            var canvas = layoutsurface.Canvas;
            if (canvas != null && Opacity>0)
            {

                var flip = AutoFlipRTL && layoutsurface.Device.FlowDirection == SkiaSharp.TextBlocks.Enum.FlowDirection.RightToLeft;
                var matrix = flip ?
                    SKMatrix.CreateScale(-1, 1).PostConcat(SKMatrix.CreateTranslation(x + picturesize.Width, y)) :
                    SKMatrix.CreateTranslation(x, y);

                if (Opacity == 1)
                {
                    canvas.DrawPicture(SKPicture, ref matrix);
                }
                else
                {
                    using (var paint = new SKPaint() { Color = SKColors.Black.WithOpacity(Opacity) })
                    {
                        canvas.DrawPicture(SKPicture, ref matrix, paint);
                    }

                }
            }

            return new SKRect(x, y, x + picturesize.Width, y + picturesize.Height);
        }


    }
}
