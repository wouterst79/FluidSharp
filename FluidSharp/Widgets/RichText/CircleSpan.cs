using FluidSharp.Layouts;
using SkiaSharp;
using SkiaSharp.TextBlocks;
using System;
using System.Collections.Generic;
using System.Text;

namespace FluidSharp.Widgets
{
    public class CircleSpan : OwnerDrawnRichTextSpan
    {

        public float Size;
        public Margins Margins;
        public SKColor Color;

        private const bool Antialias = true;

        public CircleSpan(float size, Margins margins, SKColor color)
        {
            Size = size;
            Margins = margins;
            Color = color;
        }

        public override SKSize GetSize(TextShaper textShaper) => new SKSize(Margins.TotalX + Size, Margins.TotalY + Size);

        public override void DrawMeasuredSpan(SKCanvas canvas, float x, float y, float fontheight, float marginy, MeasuredSpan measuredSpan, bool isrtl)
        {

            var radius = Size / 2;

            x += isrtl ? Margins.Far : Margins.Near + radius;
            y -= (Margins.Bottom + radius);

            if (Color != null && Color.Alpha != 0)
                using (var paint = new SKPaint() { Color = Color, IsAntialias = Antialias })
                {
                    canvas.DrawCircle(x, y, radius, paint);
                }

        }

    }
}
