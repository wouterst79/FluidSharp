using SkiaSharp;
using SkiaSharp.TextBlocks;
using System;
using System.Collections.Generic;
using System.Text;

namespace FluidSharp.Widgets
{
    public class PictureRichTextSpan : OwnerDrawnRichTextSpan
    {

        public Picture Picture;
        public float Opacity = 1;

        public PictureRichTextSpan(Picture picture) 
        {
            Picture = picture ?? throw new ArgumentNullException(nameof(picture));
        }

        public override SKSize GetSize(TextShaper textShaper) => Picture.Size;

        public override void DrawMeasuredSpan(SKCanvas canvas, float x, float y, float fontheight, float marginy, MeasuredSpan measuredSpan, bool isrtl)
        {

            var size = Picture.Size;
            y -= (fontheight + size.Height) / 2;// align center of picture to center of font

            var flip = Picture.AutoFlipRTL && isrtl;
            var matrix = flip ?
                SKMatrix.CreateScale(-1, 1).PostConcat(SKMatrix.CreateTranslation(x + size.Width, y)) :
                SKMatrix.CreateTranslation(x, y);

            if (Opacity == 1)
            {
                canvas.DrawPicture(Picture.SKPicture, ref matrix);
            }
            else
            {
                using (var paint = new SKPaint() { Color = SKColors.Black.WithOpacity(Opacity) })
                {
                    canvas.DrawPicture(Picture.SKPicture, ref matrix, paint);
                }

            }

        }

    }
}
