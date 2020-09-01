using FluidSharp.Layouts;
using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Text;

namespace FluidSharp.Widgets
{
    public class RelativeMargin : Widget
    {

        public float XFixed { get; set; }
        public float YFixed { get; set; }

        public float XMarginPercentage { get; set; }
        public float YMarginPercentage { get; set; }

        public Widget InnerWidget { get; set; }

        public RelativeMargin(float xFixed, float yFixed, float xMarginPercentage, float yMarginPercentage, Widget innerWidget)
        {
            XFixed = xFixed;
            YFixed = yFixed;
            XMarginPercentage = xMarginPercentage;
            YMarginPercentage = yMarginPercentage;
            InnerWidget = innerWidget;
        }

        private SKSize GetDxDy(SKSize available)
        {
            var availableX = (available.Width - XFixed);
            var availableY = (available.Height - YFixed);
            var dx = availableX * XMarginPercentage / 2;
            var dy = availableY * YMarginPercentage / 2;
            return new SKSize(dx, dy);
        }

        public override SKSize Measure(MeasureCache measureCache, SKSize boundaries)
        {
            var dxdy = GetDxDy(boundaries);
            var childsize = InnerWidget.Measure(measureCache, new SKSize(boundaries.Width - dxdy.Width * 2, boundaries.Height - dxdy.Height * 2));
            return new SKSize(childsize.Width + dxdy.Width * 2, childsize.Height + dxdy.Height * 2);
        }

        public override SKRect PaintInternal(LayoutSurface layoutsurface, SKRect rect)
        {
            var dxdy = GetDxDy(rect.Size);
            var childrect = new SKRect(rect.Left + dxdy.Width, rect.Top + dxdy.Height, rect.Right - dxdy.Width, rect.Bottom - dxdy.Height);
            var result = layoutsurface.Paint(InnerWidget, childrect);
            return new SKRect(rect.Left - dxdy.Width, rect.Top - dxdy.Height, rect.Right + dxdy.Width, rect.Bottom + dxdy.Height);
        }

    }
}
