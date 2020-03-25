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

        public enum HorizontalAlignment
        {
            Near,
            Center,
            Far
        }

        public enum VerticalAlignment
        {
            Top,
            Center,
            Baseline, // Direct Text children only
            Bottom
        }

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

        public static Align TopNear(Text child) => new Align(HorizontalAlignment.Near, VerticalAlignment.Top, child);
        public static Align CenterNear(Text child) => new Align(HorizontalAlignment.Near, VerticalAlignment.Center, child);
        public static Align BaselineNear(Text child) => new Align(HorizontalAlignment.Near, VerticalAlignment.Baseline, child);
        public static Align BottomNear(Widget child) => new Align(HorizontalAlignment.Near, VerticalAlignment.Bottom, child);

        public static Align TopCenter(Widget child) => new Align(HorizontalAlignment.Center, VerticalAlignment.Top, child);
        public static Align Center(Widget child) => new Align(HorizontalAlignment.Center, VerticalAlignment.Center, child);
        public static Align BaselineCenter(Widget child) => new Align(HorizontalAlignment.Center, VerticalAlignment.Baseline, child);
        public static Align BottomCenter(Widget child) => new Align(HorizontalAlignment.Center, VerticalAlignment.Bottom, child);

        public static Align TopFar(Widget child) => new Align(HorizontalAlignment.Far, VerticalAlignment.Top, child);
        public static Align CenterFar(Text child) => new Align(HorizontalAlignment.Far, VerticalAlignment.Center, child);
        public static Align BaselineFar(Text child) => new Align(HorizontalAlignment.Far, VerticalAlignment.Baseline, child);
        public static Align BottomFar(Widget child) => new Align(HorizontalAlignment.Far, VerticalAlignment.Bottom, child);


        public override SKSize Measure(MeasureCache measureCache, SKSize boundaries)
        {
            return Child.Measure(measureCache, boundaries);
        }

        public override SKRect PaintInternal(LayoutSurface layoutsurface, SKRect rect)
        {

            var childsize = Child.Measure(layoutsurface.MeasureCache, new SKSize(rect.Width, rect.Height));

            var w = childsize.Width;
            var h = childsize.Height;

            float x;
            if (layoutsurface.Device.FlowDirection == FlowDirection.LeftToRight)
            {
                if (Horizontal == HorizontalAlignment.Near)
                    x = rect.Left + Margin.Width;
                else if (Horizontal == HorizontalAlignment.Center)
                    x = rect.Left + (rect.Width - w) / 2;
                else //if (Horizontal == HorizontalAlignment.Far)
                    x = rect.Right - w - Margin.Width;
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
                if (Vertical == VerticalAlignment.Baseline && Child is Text t)
                    y = rect.Bottom - h + t.MarginY - Margin.Height;
                else
                    y = rect.Bottom - h - Margin.Height;
            }

            var childrect = new SKRect(x, y, x + w, y + w);
            layoutsurface.Paint(Child, childrect);

            return rect;
        }

    }

}
