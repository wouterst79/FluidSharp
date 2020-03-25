using FluidSharp.Layouts;
using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Text;

namespace FluidSharp.Widgets
{

    public class Center : Widget
    {

        public Widget Child;

        public Center(Widget child)
        {
            Child = child ?? throw new ArgumentNullException(nameof(child));
        }

        public override SKSize Measure(MeasureCache measureCache, SKSize boundaries)
        {
            return Child.Measure(measureCache, boundaries);
        }

        public override SKRect PaintInternal(LayoutSurface layoutsurface, SKRect rect)
        {

            var childsize = Child.Measure(layoutsurface.MeasureCache, new SKSize(rect.Width, rect.Height));

            var xm = rect.Left + rect.Width / 2;
            var ym = rect.Top + rect.Height / 2;

            var childrect = new SKRect(xm - childsize.Width / 2, ym - childsize.Height / 2, xm + childsize.Width / 2, ym + childsize.Height / 2);
            layoutsurface.Paint(Child, childrect);

            return rect;
        }

    }

}
