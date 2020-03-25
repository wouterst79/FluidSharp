using FluidSharp.Interop;
using FluidSharp.Layouts;
using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Text;

namespace FluidSharp.Widgets
{
    public class Scale : Widget
    {

        public SKPoint Factor;

        public Widget Child;

        public Scale(float factor, Widget child)
        {
            Factor = new SKPoint(factor, factor);
            Child = child;
        }

        public Scale(float scaleX, float scaleY, Widget child)
        {
            Factor = new SKPoint(scaleX, scaleY);
            Child = child;
        }

        public override SKSize Measure(MeasureCache measureCache, SKSize boundaries)
        {
            var scaledboundaries = new SKSize(boundaries.Width / Factor.X, boundaries.Height / Factor.Y);
            var childmeasure = Child.Measure(measureCache, scaledboundaries);
            return new SKSize(childmeasure.Width * Factor.X, childmeasure.Height * Factor.Y);
        }

        public override SKRect PaintInternal(LayoutSurface layoutsurface, SKRect rect)
        {
            var canvas = layoutsurface.Canvas;

            var prevmanager = layoutsurface.MeasureCache.NativeViewManager;
            if (prevmanager != null)
                layoutsurface.MeasureCache.NativeViewManager = new ScaledNativeViewManager(Factor, prevmanager);

            if (canvas != null)
            {
                canvas.Save();
                canvas.Scale(Factor);
            }
            var drawrect = new SKRect(rect.Left / Factor.X, rect.Top / Factor.Y, rect.Right / Factor.X, rect.Bottom / Factor.Y);
            var childrect = layoutsurface.Paint(Child, drawrect);
            var result = new SKRect(childrect.Left * Factor.X, childrect.Top * Factor.Y, childrect.Right * Factor.X, childrect.Bottom * Factor.Y);

            layoutsurface.MeasureCache.NativeViewManager = prevmanager;
            if (canvas != null)
            {
                canvas.Restore();
            }
            return result;
        }
    }
}
