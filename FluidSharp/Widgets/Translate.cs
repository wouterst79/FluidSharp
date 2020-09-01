using FluidSharp.Layouts;
using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Text;

namespace FluidSharp.Widgets
{
    public class Translate : Widget
    {

        public SKPoint Translation { get; set; }
        public Widget InnerWidget { get; set; }


        public Translate(float x, float y, Widget innerWidget)
        {
            Translation = new SKPoint(x, y);
            InnerWidget = innerWidget;
        }

        public Translate(SKPoint translation, Widget innerWidget)
        {
            Translation = translation;
            InnerWidget = innerWidget;
        }

        public override SKSize Measure(MeasureCache measureCache, SKSize boundaries) => InnerWidget.Measure(measureCache, boundaries);
        public override SKRect PaintInternal(LayoutSurface layoutsurface, SKRect rect)
        {
            var translated = new SKRect(rect.Left + Translation.X, rect.Top + Translation.Y, rect.Right + Translation.X, rect.Bottom + Translation.Y);
            var location = layoutsurface.Paint(InnerWidget, translated);
            var inverse = new SKRect(location.Left - Translation.X, location.Top - Translation.Y, location.Right - Translation.X, location.Bottom - Translation.Y);
            return inverse;
        }

    }
}
