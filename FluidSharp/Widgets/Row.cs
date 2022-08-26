#if DEBUG
//#define DEBUGCONTAINER
#endif
using FluidSharp.Layouts;
using FluidSharp.Widgets.Members;
using SkiaSharp;
using SkiaSharp.TextBlocks.Enum;
using System;
using System.Collections.Generic;
using System.Text;

namespace FluidSharp.Widgets
{

    public class Row : Widget
    {

        public Margins Margin;
        public float MinimumHeight;
        public float Spacing;
        public bool ExpandHorizontal = false;
        public VerticalAlignment VerticalChildAlignment = VerticalAlignment.Top;

        public FixableList<Widget?> Children;

        public Row(float spacing, VerticalAlignment verticalChildAlignment, params Widget?[] widgets)
        {
            Spacing = spacing;
            VerticalChildAlignment = verticalChildAlignment;
            Children = new FixableList<Widget?>(widgets);
        }

        public Row(float spacing, params Widget?[] widgets)
        {
            Spacing = spacing;
            Children = new FixableList<Widget?>(widgets);
        }

        public override SKSize Measure(MeasureCache measureCache, SKSize boundaries)
        {
            var childboundaries = new SKSize(
                                                boundaries.Width - Margin.TotalX,
                                                boundaries.Height - Margin.TotalY
                                            );
            var (w, h, measures) = LayoutChildren(measureCache, childboundaries);
            return new SKSize(w + Margin.TotalX, h + Margin.TotalY);
        }

        public override SKRect PaintInternal(LayoutSurface layoutsurface, SKRect rect)
        {

            var innerrect = Margin.Shrink(rect, layoutsurface.Device.FlowDirection);

            var (width, height, measures) = LayoutChildren(layoutsurface.MeasureCache, innerrect.Size);

            var t = innerrect.Top;
            var b = innerrect.Top + height;

            var flowdirection = layoutsurface.Device.FlowDirection;
            var x = flowdirection == FlowDirection.LeftToRight ? innerrect.Left : innerrect.Right;

            for (int i = 0; i < measures.Count; i++)
            {

                var (child, childsize) = measures[i];
                var w = childsize.Width;
                var h = childsize.Height;

                float y;
                switch (VerticalChildAlignment)
                {
                    case VerticalAlignment.Top: y = t; break;
                    case VerticalAlignment.Center: y = t + (height - h) / 2; break;
                    case VerticalAlignment.Bottom: y = b - h; break;
                    default: throw new NotSupportedException($"VerticalChildAlignment {VerticalChildAlignment} is not supported");
                }

                if (flowdirection == FlowDirection.LeftToRight)
                {
                    var childrect = new SKRect(x, y, x + w, y + h);
                    layoutsurface.Paint(child, childrect);

#if DEBUGCONTAINER
                    layoutsurface.DebugRect(childrect, SKColors.Green.WithAlpha(128));
#endif
                    x += w + Spacing;

                }
                else
                {
                    var childrect = new SKRect(x - w, y, x, y + h);

                    layoutsurface.Paint(child, childrect);

#if DEBUGCONTAINER
                    layoutsurface.DebugRect(childrect, SKColors.Green.WithAlpha(128));
#endif
                    x -= w + Spacing;
                }

            }


            SKRect myrect;
            if (flowdirection == FlowDirection.LeftToRight)
                myrect = new SKRect(innerrect.Left, t, innerrect.Left + width, b);
            else
                myrect = new SKRect(innerrect.Right - width, t, innerrect.Right, b);

#if DEBUGCONTAINER
            layoutsurface.DebugRect(myrect, SKColors.Red.WithAlpha(128));
#endif

            myrect = Margin.Grow(myrect, flowdirection);

#if DEBUGCONTAINER
            layoutsurface.DebugRect(myrect, SKColors.Purple.WithAlpha(128));
#endif

            return myrect;
        }

        private (float width, float height, List<(Widget widget, SKSize measure)>) LayoutChildren(MeasureCache measureCache, SKSize boundaries)
        {

            Children.IsFixed = true;

            var measures = new List<(Widget widget, SKSize size)>();

            var w = 0f;
            var h = MinimumHeight - Margin.TotalY;

            if (Children != null)
                foreach (var child in Children)
                    if (child != null)
                    {

                        var available = ExpandHorizontal ? new SKSize(boundaries.Width - w, boundaries.Height) : boundaries;
                        var measure = child.Measure(measureCache, available);
                        w = w + measure.Width + Spacing;
                        if (h < measure.Height) h = measure.Height;
                        measures.Add((child, measure));

                    }

            if (w > 0) w -= Spacing;

            if (ExpandHorizontal)
                w = boundaries.Width;

            return (w, h, measures);

        }

    }
}
