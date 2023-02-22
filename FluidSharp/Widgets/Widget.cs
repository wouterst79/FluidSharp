using FluidSharp.Layouts;
using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace FluidSharp.Widgets
{
    public abstract class Widget
    {


        public static Action<Widget>? WidgetAllocated;

#if DEBUG
        public string? DebugTag;
        public bool IsNew = true;
#endif

        public abstract SKSize Measure(MeasureCache measureCache, SKSize boundaries);

        public abstract SKRect PaintInternal(LayoutSurface layoutsurface, SKRect rect);


        public Widget()
        {
            WidgetAllocated?.Invoke(this);
        }

    }
}
