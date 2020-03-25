using FluidSharp.Layouts;
using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Text;

namespace FluidSharp.Widgets.Native
{
    public abstract class NativeViewWidget : Widget
    {

        public override SKSize Measure(MeasureCache measureCache, SKSize boundaries)
        {
            var childsize = measureCache.NativeViewManager.Measure(this, boundaries);
            return childsize;
        }

        public override SKRect PaintInternal(LayoutSurface layoutsurface, SKRect rect)
        {
            if (layoutsurface.Canvas != null)
            {
                layoutsurface.MeasureCache.NativeViewManager.UpdateNativeView(this, rect);
            }
            return rect;
        }

    }
}
