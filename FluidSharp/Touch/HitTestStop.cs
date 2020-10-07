using FluidSharp.Layouts;
using FluidSharp.Widgets;
using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Text;

namespace FluidSharp.Touch
{
    public class HitTestStop : Widget
    {

        public Widget? Contents;

        public HitTestStop(Widget? contents = null) => Contents = contents;

        public override SKSize Measure(MeasureCache measureCache, SKSize boundaries) => Contents == null ? new SKSize(0, 0) : Contents.Measure(measureCache, boundaries);
        public override SKRect PaintInternal(LayoutSurface layoutsurface, SKRect rect) => Contents == null ? rect.WithHeight(0) : layoutsurface.Paint(Contents, rect);

    }
}
