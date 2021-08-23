#define SHOWSPACING
using FluidSharp.Layouts;
using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Text;

namespace FluidSharp.Widgets
{

    public class SplitContainerFar : Widget
    {

        public Widget Near { get; set; }
        public float Spacing { get; set; }
        public Widget Far { get; set; }

        public float Height { get; set; }

        public Margins Margin;

        public SplitContainerFar(Widget near, float spacing, Widget far)
        {
            Near = near ?? throw new ArgumentNullException(nameof(near));
            Spacing = spacing;
            Far = far ?? throw new ArgumentNullException(nameof(far));
        }

        public SplitContainerFar(Widget near, float spacing, Widget far, float height, Margins margin)
        {
            Near = near ?? throw new ArgumentNullException(nameof(near));
            Spacing = spacing;
            Far = far ?? throw new ArgumentNullException(nameof(far));
            Height = height;
            Margin = margin;
        }

        public override SKSize Measure(MeasureCache measureCache, SKSize boundaries)
        {
            if (Height == 0)
            {
                var nearheight = Near.Measure(measureCache, boundaries).Height;
                return new SKSize(boundaries.Width, nearheight);
            }
            return new SKSize(boundaries.Width, Height);
        }

        public override SKRect PaintInternal(LayoutSurface layoutsurface, SKRect rect)
        {

            var height = Height;
            if (height == 0)
            {
                height = Near.Measure(layoutsurface.MeasureCache, rect.Size).Height;
            }

            var childrect = Margin.Shrink(rect, layoutsurface.FlowDirection).WithHeight(height - Margin.TotalY);
            var farwidth = Far.Measure(layoutsurface.MeasureCache, childrect.Size).Width;

            // near
            var nearwidth = childrect.Width - Spacing - farwidth;
            if (nearwidth>0)
            {
                var nearrect = childrect.HorizontalAlign(new SKSize(nearwidth, childrect.Height), HorizontalAlignment.Near, layoutsurface.FlowDirection);
                layoutsurface.Paint(Near, nearrect);
            }

            // far
            var farrect = childrect.HorizontalAlign(new SKSize(farwidth, childrect.Height), HorizontalAlignment.Far, layoutsurface.FlowDirection);
            layoutsurface.Paint(Far, farrect);

#if SHOWSPACING
            layoutsurface.DebugMargin(childrect, Margin, SKColors.YellowGreen);
            //layoutsurface.DebugRect(drawrect, SKColors.Blue.WithAlpha(128));
#endif

            return rect.WithHeight(height);

        }
    }
}
