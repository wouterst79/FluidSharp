using SkiaSharp;
using SkiaSharp.TextBlocks;
using System;
using System.Collections.Generic;
using System.Text;

namespace FluidSharp.Widgets
{
    public class PictureSpan : OwnerDrawnRichTextSpan
    {

        public Picture Picture;
        public float Opacity = 1;
        public SKPoint Translate;

        public PictureSpan(Picture picture)
        {
            Picture = picture ?? throw new ArgumentNullException(nameof(picture));
        }

        public override SKSize GetSize(TextShaper textShaper) => Picture.Size;

        public override void DrawMeasuredSpan(SKCanvas canvas, float x, float y, float fontheight, float marginy, MeasuredSpan measuredSpan, bool isrtl)
        {

            var size = Picture.Size;
            y -= (fontheight + size.Height) / 2;// align center of picture to center of font

            var rect = new SKRect(x, y, x + size.Width, y + size.Height);
            Picture.Paint(canvas, rect, isrtl);

        }

    }
}
