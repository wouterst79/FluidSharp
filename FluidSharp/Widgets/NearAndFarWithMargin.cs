#if DEBUG
#define SHOWMARGIN
#endif
using FluidSharp.Layouts;
using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Net.WebSockets;
using System.Text;

namespace FluidSharp.Widgets
{

    /// <summary>
    /// A container with 2 widgets:
    ///     Near = near-center aligned,
    ///     Far = far-center aligned.
    /// Widgets will not overlap; width is assigned relative to full width request
    /// </summary>
    public class NearAndFarWithMargin : Widget
    {

        public Margins Margin;
        public Widget? Near;
        public Widget? Far;

        public SKSize MinimumSize;

        public NearAndFarWithMargin(Margins margin, Widget? near, Widget? far)
        {
            Margin = margin;
            Near = near;
            Far = far;
        }

        public override SKSize Measure(MeasureCache measureCache, SKSize boundaries)
        {
            var sizes = Layout(measureCache, Margin.Shrink(boundaries));
            var result = Margin.Grow(new SKSize(boundaries.Width, sizes.height));
            if (result.Width < MinimumSize.Width) result.Width = MinimumSize.Width;
            if (result.Height < MinimumSize.Height) result.Height = MinimumSize.Height;
            return result;
        }

        public override SKRect PaintInternal(LayoutSurface layoutsurface, SKRect rect)
        {

            var childrect = Margin.Shrink(rect, layoutsurface.Device.FlowDirection);

            var sizes = Layout(layoutsurface.MeasureCache, childrect.Size);

            var isrtl = layoutsurface.Device.FlowDirection == SkiaSharp.TextBlocks.Enum.FlowDirection.RightToLeft;

            // paint left widget
            var leftwidget = isrtl ? Far : Near;
            if (leftwidget != null)
            {
                var lefttop = isrtl ? sizes.fartop : sizes.neartop;
                var leftwidth = isrtl ? sizes.farwidth : sizes.nearwidth;
                var leftrect = new SKRect(childrect.Left, childrect.Top + lefttop, childrect.Left + leftwidth, childrect.Top + sizes.height);
                layoutsurface.Paint(leftwidget, leftrect);
            }

            // paint right widget
            var rightwidget = isrtl ? Near : Far;
            if (rightwidget != null)
            {
                var righttop = isrtl ? sizes.neartop : sizes.fartop;
                var rightwidth = isrtl ? sizes.nearwidth : sizes.farwidth;
                var rightrect = new SKRect(childrect.Right - rightwidth, childrect.Top + righttop, childrect.Right, childrect.Top + sizes.height);
                layoutsurface.Paint(rightwidget, rightrect);
            }

            rect = childrect.WithHeight(sizes.height);

#if SHOWMARGIN
            layoutsurface.DebugMargin(rect, Margin, SKColors.Red);
#endif

            return Margin.Grow(rect, layoutsurface.Device.FlowDirection);
        }

        private (float nearwidth, float neartop, float farwidth, float fartop, float height) Layout(MeasureCache measureCache, SKSize boundaries)
        {

            var s1 = Near == null ? new SKSize(0, 0) : Near.Measure(measureCache, boundaries);
            var s2 = Far == null ? new SKSize(0, 0) : Far.Measure(measureCache, boundaries);

            if (s1.Width + s2.Width > boundaries.Width)
            {

                // overflow, assign widths relative to original request
                var pct = s1.Width / (s1.Width + s2.Width);

                var w1 = pct * boundaries.Width;
                var w2 = boundaries.Width - w1;

                s1 = Near == null ? new SKSize(0, 0) : Near.Measure(measureCache, new SKSize(w1, boundaries.Height));
                s2 = Far == null ? new SKSize(0, 0) : Far.Measure(measureCache, new SKSize(w2, boundaries.Height));

            }

            var h = MathF.Max(s1.Height, s2.Height);

            // v-center near and far
            return (s1.Width, (h - s1.Height) / 2, s2.Width, (h - s2.Height) / 2, h);

        }

    }
}
