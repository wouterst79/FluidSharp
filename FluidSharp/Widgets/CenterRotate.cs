using FluidSharp.Layouts;
using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Text;

namespace FluidSharp.Widgets
{
    public class CenterRotate : Widget
    {

        public float Degrees { get; set; }
        public Widget InnerWidget { get; set; }

        public CenterRotate(float degrees, Widget innerWidget)
        {
            Degrees = degrees;
            InnerWidget = innerWidget;
        }

        public override SKSize Measure(MeasureCache measureCache, SKSize boundaries) => InnerWidget.Measure(measureCache, boundaries);
        public override SKRect PaintInternal(LayoutSurface layoutsurface, SKRect rect)
        {
            var canvas = layoutsurface.Canvas;
            if (canvas != null)
            {
                canvas.Save();
                canvas.RotateDegrees(Degrees, rect.MidX, rect.MidY);
            }

            var result = layoutsurface.Paint(InnerWidget, rect);

            if (canvas != null)
                canvas.Restore();

            return result;

        }

    }
}
