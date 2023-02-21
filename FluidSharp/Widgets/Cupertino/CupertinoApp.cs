using FluidSharp.Layouts;
using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Text;

namespace FluidSharp.Widgets.Cupertino
{
    public class CupertinoApp : Widget
    {

        public Widget Child;

        public CupertinoApp(Widget contents)
        {
            Child = new Layout()
            {
                Rows =
                {
                    LayoutSize.Absolute(30),
                    LayoutSize.Remaining
                },
                Cells =
                {
                    new LayoutCell(0,1, contents)
                }
            };
        }

        public override SKSize Measure(MeasureCache measureCache, SKSize boundaries) => Child.Measure(measureCache, boundaries);
        public override SKRect PaintInternal(LayoutSurface layoutsurface, SKRect rect) => layoutsurface.Paint(Child, rect);

    }
}
