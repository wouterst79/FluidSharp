using FluidSharp.Widgets;
using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Text;

namespace FluidSharp.Touch
{
    public class HitTestHit
    {

        public Device Device;
        public Widget Widget;
        public SKPoint LocationInWidget;
        public SKRect WidgetRect;
        public SKPoint Scale;

        public HitTestHit(Device device, Widget widget, SKPoint locationInWidget, SKRect widgetRect, SKPoint scale)
        {
            Device = device;
            Widget = widget;
            LocationInWidget = locationInWidget;
            WidgetRect = widgetRect;
            Scale = scale;
        }

    }
}
