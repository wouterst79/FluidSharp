using FluidSharp.Layouts;
using SkiaSharp;
using SkiaSharp.TextBlocks.Enum;
using System;
using System.Collections.Generic;
using System.Text;

namespace FluidSharp.Widgets
{

    public class Align : Widget
    {

        public HorizontalAlignment Horizontal;
        public VerticalAlignment Vertical;

        public SKSize Margin;

        public Widget Child;


        public Align(HorizontalAlignment horizontal, VerticalAlignment vertical, Widget child)
            : this(horizontal, vertical, new SKSize(), child)
        {
        }

        public Align(HorizontalAlignment horizontal, VerticalAlignment vertical, SKSize margin, Widget child)
        {
            Horizontal = horizontal;
            Vertical = vertical;
            Margin = margin;
            Child = child ?? throw new ArgumentNullException(nameof(child));
        }

        public static Align TopNear(Widget child, SKSize margins = default) => new Align(HorizontalAlignment.Near, VerticalAlignment.Top, margins, child);
        public static Align CenterNear(Widget child, SKSize margins = default) => new Align(HorizontalAlignment.Near, VerticalAlignment.Center, margins, child);
        public static Align BaselineNear(ITextWidget child, SKSize margins = default) => new Align(HorizontalAlignment.Near, VerticalAlignment.Baseline, margins, (Widget)child);
        public static Align BottomNear(Widget child, SKSize margins = default) => new Align(HorizontalAlignment.Near, VerticalAlignment.Bottom, margins, child);

        public static Align TopCenter(Widget child, SKSize margins = default) => new Align(HorizontalAlignment.Center, VerticalAlignment.Top, margins, child);
        public static Align Center(Widget child) => new Align(HorizontalAlignment.Center, VerticalAlignment.Center, child);
        public static Align Center(Widget child, SKSize allmargins) => new Align(HorizontalAlignment.Center, VerticalAlignment.Center, new SKSize(allmargins.Width * 2, allmargins.Height * 2), child);
        public static Align BaselineCenter(ITextWidget child, SKSize margins = default) => new Align(HorizontalAlignment.Center, VerticalAlignment.Baseline, margins, (Widget)child);
        public static Align BottomCenter(Widget child, SKSize margins = default) => new Align(HorizontalAlignment.Center, VerticalAlignment.Bottom, margins, child);
        public static Align Bottom(Widget child, SKSize margins = default) => new Align(HorizontalAlignment.Expand, VerticalAlignment.Bottom, margins, child);

        public static Align TopFar(Widget child, SKSize margins = default) => new Align(HorizontalAlignment.Far, VerticalAlignment.Top, margins, child);
        public static Align CenterFar(Widget child, SKSize margins = default) => new Align(HorizontalAlignment.Far, VerticalAlignment.Center, margins, child);
        public static Align BaselineFar(ITextWidget child, SKSize margins = default) => new Align(HorizontalAlignment.Far, VerticalAlignment.Baseline, margins, (Widget)child);
        public static Align BottomFar(Widget child, SKSize margins = default) => new Align(HorizontalAlignment.Far, VerticalAlignment.Bottom, margins, child);


        public override SKSize Measure(MeasureCache measureCache, SKSize boundaries)
        {
            var available = boundaries - Margin;
            var childsize = Child.Measure(measureCache, available);
            return new SKSize(childsize.Width + Margin.Width, childsize.Height + Margin.Height);
        }

        public SKRect GetChildRect(LayoutSurface layoutsurface, SKRect rect)
        {

            var available = rect.Size - Margin;
            var childsize = Child.Measure(layoutsurface.MeasureCache, available);

            var w = childsize.Width;
            var h = childsize.Height;

            float x;
            if (layoutsurface.Device.FlowDirection == FlowDirection.LeftToRight)
            {
                if (Horizontal == HorizontalAlignment.Near)
                    x = rect.Left + Margin.Width;
                else if (Horizontal == HorizontalAlignment.Center)
                    x = rect.Left + (rect.Width - w) / 2;
                else if (Horizontal == HorizontalAlignment.Far)
                    x = rect.Right - w - Margin.Width;
                else if (Horizontal == HorizontalAlignment.Expand)
                {
                    x = rect.Left;
                    w = rect.Width;
                }
                else throw new ArgumentOutOfRangeException(nameof(Horizontal));
            }
            else
            {
                if (Horizontal == HorizontalAlignment.Near)
                    x = rect.Right - w - Margin.Width;
                else if (Horizontal == HorizontalAlignment.Center)
                    x = rect.Left + (rect.Width - w) / 2;
                else //if (Horizontal == HorizontalAlignment.Far)
                    x = rect.Left + Margin.Width;
            }


            float y;
            if (Vertical == VerticalAlignment.Top)
                y = rect.Top + Margin.Height;
            else if (Vertical == VerticalAlignment.Center)
                y = rect.Top + (rect.Height - h) / 2;
            else
            {
                if (Vertical == VerticalAlignment.Baseline && Child is ITextWidget t)
                    y = rect.Bottom - h + t.GetMarginY() - Margin.Height;
                else
                    y = rect.Bottom - h - Margin.Height;
            }

            return new SKRect(x, y, x + w, y + h);

        }

        public override SKRect PaintInternal(LayoutSurface layoutsurface, SKRect rect)
        {
            var childrect = GetChildRect(layoutsurface, rect);
            layoutsurface.Paint(Child, childrect);
            return rect;
        }

    }

}
