#if DEBUG
//#define DEBUGCONTAINER
#endif
using FluidSharp.Layouts;
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
        public bool ExpandHorizontal = true;
        public VerticalAlignment VerticalChildAlignment;

        public List<Widget> Children = new List<Widget>();

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

            var measures = new List<(Widget widget, SKSize size)>();

            var w = 0f;
            var h = MinimumHeight - Margin.TotalY;

            if (Children != null)
                foreach (var child in Children)
                    if (child != null)
                    {

                        var measure = child.Measure(measureCache, boundaries);
                        w = w + measure.Width + Spacing;
                        if (h < measure.Height) h = measure.Height;
                        measures.Add((child, measure));

                    }

            if (w > 0) w -= Spacing;


            if (w > boundaries.Width)
            {

                // overflow, take the widest widget, and supply only available width (IE boundary - total width of other children)
                // problem: this may make the large widget align to 3 lines, while a wider widget now is only 1 line.
                // maybe this is ok. So, just trying it out for now. if it's funky, do something about it.

                // another problem may be that the widest widget has fixed dimensions. 

                // another problem is that with this algorithm, available can be <=0

                // determine widest child
                Widget widest = null;
                float maxw = 0;
                int widestid = -1;
                for (int i = 0; i < measures.Count; i++)
                {
                    var (c, s) = measures[i];
                    if (s.Width > maxw)
                    {
                        widest = c;
                        maxw = s.Width;
                        widestid = i;
                    }
                }

                // determine available width
                var available = boundaries.Width - Spacing * (measures.Count - 1);
                for (int i = 0; i < measures.Count; i++)
                {
                    var (c, s) = measures[i];
                    if (c != widest)
                        available -= s.Width;
                }

                if (available < 0)
                {

                }
                else
                {

                }

                if (widest != null)
                {
                    var measure = widest.Measure(measureCache, new SKSize(available, boundaries.Height));
                    measures[widestid] = (widest, measure);

                    if (h < measure.Height) h = measure.Height;

                }

            }

            if (ExpandHorizontal)
                w = boundaries.Width;

            return (w, h, measures);

        }

    }
}
