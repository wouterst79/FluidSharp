﻿#if DEBUG
//#define DEBUGCONTAINER
#endif
using FluidSharp.Layouts;
using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Text;

namespace FluidSharp.Widgets
{
    public class Column : Widget
    {

#if DEBUGCONTAINER
        public bool Debug;
#endif

        public float Spacing;
        public Widget Separator;
        public bool ExpandHorizontal = true;

        public List<Widget> Children = new List<Widget>();

        public Column() { }
        public Column(params Widget[] widgets) { Children.AddRange(widgets); }

        public override SKSize Measure(MeasureCache measureCache, SKSize boundaries)
        {

            var w = 0f;
            var h = 0f;

            if (Separator != null)
            {
                var separatorsize = Separator.Measure(measureCache, boundaries);
                if (w < separatorsize.Width) w = separatorsize.Width;
                Spacing = separatorsize.Height;
            }

            var any = false;
            if (Children != null)
                foreach (var child in Children)
                    if (child != null)
                    {
                        var childsize = child.Measure(measureCache, boundaries);
                        if (w < childsize.Width) w = childsize.Width;
                        h += childsize.Height + Spacing;
                        any = true;
                    }

            if (any)
                h -= Spacing;

            if (ExpandHorizontal)
                w = boundaries.Width;

            return new SKSize(w, h);

        }

        public override SKRect PaintInternal(LayoutSurface layoutsurface, SKRect rect)
        {

            Widget lastchild = null;
            if (Separator != null)
            {
                Spacing = Separator.Measure(layoutsurface.MeasureCache, rect.Size).Height;
                foreach (var child in Children)
                    if (child != null)
                        lastchild = child;
            }

            var l = rect.Left;
            var y = rect.Top;
            var r = rect.Right;
            var b = rect.Bottom;

            if (Children != null)
                foreach (var child in Children)
                    if (child != null)
                    {

                        var childrect = new SKRect(l, y, r, b);
                        var painted = layoutsurface.Paint(child, childrect);

                        y += painted.Height;

                        if (child != lastchild)
                        {
                            if (Separator != null && child != lastchild)
                            {
                                layoutsurface.Paint(Separator, new SKRect(l, y, r, y + Spacing));
                            }
                            y += Spacing;
                        }

                    }


#if DEBUGCONTAINER
            if (Debug)
            {
                var measure = Measure(new SKSize(rect.Width, rect.Height));
                layoutsurface.DebugRect(rect, SKColors.Green);
                layoutsurface.DebugRect(new SKRect(rect.Left, rect.Top, rect.Left + measure.Width, rect.Top + measure.Height), SKColors.Red);
            }
#endif

            return new SKRect(l, rect.Top, r, y);

        }

    }
}
