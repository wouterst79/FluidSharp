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
    public class NearAndFar : Widget
    {

        public Widget? Near;
        public Widget? Far;

        public NearAndFar(Widget? near, Widget? far)
        {
            Near = near;
            Far = far;
        }

        public override SKSize Measure(MeasureCache measureCache, SKSize boundaries)
        {
            var sizes = Layout(measureCache, boundaries);
            return new SKSize(boundaries.Width, sizes.height);
        }

        public override SKRect PaintInternal(LayoutSurface layoutsurface, SKRect rect)
        {

            var sizes = Layout(layoutsurface.MeasureCache, rect.Size);

            var isrtl = layoutsurface.Device.FlowDirection == SkiaSharp.TextBlocks.Enum.FlowDirection.RightToLeft;

            // paint left widget
            var leftwidget = isrtl ? Far : Near;
            if (leftwidget != null)
            {
                var lefttop = isrtl ? sizes.fartop : sizes.neartop;
                var leftwidth = isrtl ? sizes.farwidth : sizes.nearwidth;
                var leftrect = new SKRect(rect.Left, rect.Top + lefttop, rect.Left + leftwidth, rect.Top + sizes.height);
                layoutsurface.Paint(leftwidget, leftrect);
            }

            // paint right widget
            var rightwidget = isrtl ? Near : Far;
            if (rightwidget != null)
            {
                var righttop = isrtl ? sizes.neartop : sizes.fartop;
                var rightwidth = isrtl ? sizes.nearwidth : sizes.farwidth;
                var rightrect = new SKRect(rect.Right - rightwidth, rect.Top + righttop, rect.Right, rect.Top + sizes.height);
                layoutsurface.Paint(rightwidget, rightrect);
            }

            return new SKRect(rect.Left, rect.Top, rect.Right, rect.Top + sizes.height);
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
