using FluidSharp.Layouts;
using FluidSharp.Paint;
using SkiaSharp;
using SkiaSharp.TextBlocks;
using System;
using System.Collections.Generic;
using System.Text;

namespace FluidSharp.Widgets
{
    public class ShapeAsFontGlyph : Widget
    {

        public Font Font;
        public Shape Shape;

        public ShapeAsFontGlyph(Font font, Shape shape)
        {
            Font = font;
            Shape = shape;
        }

        public override SKSize Measure(MeasureCache measureCache, SKSize boundaries)
        {
            var (charwidth, fontmetrics) = GetMetrics(measureCache);

            var FontHeight = fontmetrics.CapHeight;
            var LineHeight = -fontmetrics.Top;
            var MarginY = (LineHeight - FontHeight) / 2;

            return new SKSize(charwidth, LineHeight);
        }

        public override SKRect PaintInternal(LayoutSurface layoutsurface, SKRect rect)
        {
            
            var (charwidth, fontmetrics) = GetMetrics(layoutsurface.MeasureCache);

            var FontHeight = fontmetrics.CapHeight;
            var LineHeight = -fontmetrics.Top;
            var MarginY = (LineHeight - FontHeight) / 2;

            var paintrect = new SKRect(rect.Left, rect.Top + MarginY, rect.Left + charwidth, rect.Top + LineHeight - MarginY);

            layoutsurface.Paint(Shape, paintrect);

            return paintrect;

        }

        private (float width, SKFontMetrics fontmetrics) GetMetrics(MeasureCache measureCache)
        {
            var shape = measureCache.TextShaper.GetGlyphSpan(Font, "a");
            return (shape.Measure(0, 0).width, shape.Paints[0].FontMetrics);
        }


    }
}
