#if DEBUG
#define DEBUGCONTAINER
#endif
using FluidSharp.Layouts;
using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Text;

namespace FluidSharp.Widgets
{

    public class CenterHorizontal : Widget
    {

        public Widget Child;
        public Margins Margin;

        public CenterHorizontal(Widget child)
        {
            Child = child ?? throw new ArgumentNullException(nameof(child));
        }

        public override SKSize Measure(MeasureCache measureCache, SKSize boundaries)
        {
            var size = Child.Measure(measureCache, Margin.Shrink(boundaries));
            return Margin.Grow(size);
        }

        public override SKRect PaintInternal(LayoutSurface layoutsurface, SKRect rect)
        {

            var childrect = Margin.Shrink(rect, layoutsurface.FlowDirection);

            var childsize = Child.Measure(layoutsurface.MeasureCache, new SKSize(childrect.Width, childrect.Height));

            var xm = rect.MidX;

            childrect = new SKRect(xm - childsize.Width / 2, childrect.Top, xm + childsize.Width / 2, childrect.Top + childsize.Height);
            layoutsurface.Paint(Child, childrect);

#if DEBUGCONTAINER
            layoutsurface.DebugMargin(childrect, Margin, SKColors.YellowGreen);
#endif
            return rect.WithHeight(childsize.Height + Margin.TotalY);
        }

    }

}
