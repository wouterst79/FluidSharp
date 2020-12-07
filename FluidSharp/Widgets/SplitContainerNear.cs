#if DEBUG
#define DEBUGCONTAINER
#endif
using FluidSharp.Layouts;
using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Text;

namespace FluidSharp.Widgets
{

    public class SplitContainerNear : Widget
    {

        public Widget Near { get; set; }
        public float Spacing { get; set; }
        public Widget Far { get; set; }

        public float Height { get; set; }

        public Margins Margin;

        public SplitContainerNear(Widget near, float spacing, Widget far, float height, Margins margin)
        {
            Near = near ?? throw new ArgumentNullException(nameof(near));
            Spacing = spacing;
            Far = far ?? throw new ArgumentNullException(nameof(far));
            Height = height;
            Margin = margin;
        }

        public override SKSize Measure(MeasureCache measureCache, SKSize boundaries) => new SKSize(boundaries.Width, Height);
        public override SKRect PaintInternal(LayoutSurface layoutsurface, SKRect rect)
        {

            var childrect = Margin.Shrink(rect, layoutsurface.FlowDirection).WithHeight(Height - Margin.TotalY);
            var nearwidth = Near.Measure(layoutsurface.MeasureCache, childrect.Size).Width;

            // near
            var nearrect = childrect.HorizontalAlign(new SKSize(nearwidth, childrect.Height), HorizontalAlignment.Near, layoutsurface.FlowDirection);
            layoutsurface.Paint(Near, nearrect);

            // far
            var farwidth = childrect.Width - Spacing - nearwidth;
            if (farwidth>0)
            {
                var farrect = childrect.HorizontalAlign(new SKSize(farwidth, childrect.Height), HorizontalAlignment.Far, layoutsurface.FlowDirection);
                layoutsurface.Paint(Far, farrect);
            }

#if DEBUGCONTAINER
            layoutsurface.DebugMargin(childrect, Margin, SKColors.YellowGreen);
            //layoutsurface.DebugRect(drawrect, SKColors.Blue.WithAlpha(128));
#endif

            return rect.WithHeight(Height);

        }
    }
}
