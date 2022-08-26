#define SHOWSPACING
#if DEBUG
//#define DEBUGCONTAINER
#endif
using FluidSharp.Layouts;
using FluidSharp.Widgets.Members;
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
#if SHOWSPACING
        public SKColor SpacingColor = SKColors.Purple;
#endif

        public Margins Margin;

        public float Spacing;
        public Widget? Separator;
        public bool ExpandHorizontal = true;
        public HorizontalAlignment HorizontalAlignment = HorizontalAlignment.Near;

        public FixableList<Widget?> Children;

        public Column(float spacing, HorizontalAlignment horizontalAlignment = HorizontalAlignment.Near)
        {
            Spacing = spacing;
            HorizontalAlignment = horizontalAlignment;
            Children = new FixableList<Widget?>();
        }
        public Column(float spacing, params Widget?[] widgets)
        {
            Spacing = spacing;
            Children = new FixableList<Widget?>(widgets);
        }

        public override SKSize Measure(MeasureCache measureCache, SKSize boundaries)
        {

            Children.IsFixed = true;

            boundaries = Margin.Shrink(boundaries);

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
            {
                foreach (var child in Children)
                {
                    if (child is Spacing s)
                    {
                        if (any)
                            h = h - Spacing;
                        h += s.Size.Height;
                    }
                    else
                    if (child != null)
                    {
                        var childsize = child.Measure(measureCache, boundaries);
                        if (w < childsize.Width) w = childsize.Width;
                        h += childsize.Height + Spacing;
                        any = true;
                    }
                }
            }

            if (any)
                h -= Spacing;

            if (ExpandHorizontal)
                w = boundaries.Width;

            return Margin.Grow(new SKSize(w, h));

        }

        public override SKRect PaintInternal(LayoutSurface layoutsurface, SKRect rect)
        {

            Children.IsFixed = true;

            rect = Margin.Shrink(rect, layoutsurface.FlowDirection);

            Widget? lastchild = null;
            if (Separator != null)
                Spacing = Separator.Measure(layoutsurface.MeasureCache, rect.Size).Height;

            foreach (var child in Children)
                if (child != null)
                    lastchild = child;

            var l = rect.Left;
            var y = rect.Top;
            var r = rect.Right;
            var b = rect.Bottom;

            if (Children != null)
            {

                var hadchild = false;
                foreach (var child in Children)
                {
                    if (child is Spacing s)
                    {

                        if (hadchild)
                            y = y - Spacing;

#if SHOWSPACING
                        layoutsurface.DebugSpacing(new SKRect(l, y, r, y + s.Size.Height), s.Size.Height, SKColors.Blue);
#endif

                        y += s.Size.Height;

                    }
                    else
                    if (child != null)
                    {

                        var childrect = new SKRect(l, y, r, b);


                        if (HorizontalAlignment != HorizontalAlignment.Near)
                        {
                            var childsize = child.Measure(layoutsurface.MeasureCache, childrect.Size);
                            childrect = childrect.HorizontalAlign(childsize, HorizontalAlignment, layoutsurface.Device.FlowDirection);
                        }

                        var painted = layoutsurface.Paint(child, childrect);

                        y += painted.Height;

                        if (child != lastchild)
                        {
                            if (Separator != null && child != lastchild)
                            {
                                layoutsurface.Paint(Separator, new SKRect(l, y, r, y + Spacing));
                            }

#if SHOWSPACING
                            var idx = Children.IndexOf(child);
                            if (!(Children[idx + 1] is Spacing))
                                layoutsurface.DebugSpacing(new SKRect(l, y, r, y + Spacing), Spacing, SpacingColor);
#endif

                            y += Spacing;
                        }

                        hadchild = true;

                    }
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

#if SHOWSPACING
            layoutsurface.DebugMargin(new SKRect(l, rect.Top, r, y), Margin, SKColors.DarkCyan);
#endif

            return Margin.Grow(new SKRect(l, rect.Top, r, y), layoutsurface.FlowDirection);

        }

    }
}
