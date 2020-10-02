using FluidSharp.Layouts;
using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Text;

namespace FluidSharp.Widgets
{
    public class Hero : Widget
    {

        public string Tag { get; set; }
        public Widget Child { get; set; }

        public Hero(string tag, Widget child)
        {
            Tag = tag ?? throw new ArgumentNullException(nameof(tag));
            Child = child ?? throw new ArgumentNullException(nameof(child));
        }

        public override SKSize Measure(MeasureCache measureCache, SKSize boundaries) => Child.Measure(measureCache, boundaries);

        public override SKRect PaintInternal(LayoutSurface layoutsurface, SKRect rect)
        {
            return layoutsurface.Paint(Child, rect);
        }

    }
}
