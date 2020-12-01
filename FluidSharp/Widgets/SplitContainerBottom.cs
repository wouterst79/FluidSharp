using FluidSharp.Layouts;
using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Text;

namespace FluidSharp.Widgets
{

    public class SplitContainerBottom : Widget
    {

        public Widget MainContents { get; set; }
        public Widget RemainingContents { get; set; }

        public SplitContainerBottom(Widget mainContents, Widget remainingContents)
        {
            MainContents = mainContents ?? throw new ArgumentNullException(nameof(mainContents));
            RemainingContents = remainingContents ?? throw new ArgumentNullException(nameof(remainingContents));
        }

        public override SKSize Measure(MeasureCache measureCache, SKSize boundaries) => boundaries;
        public override SKRect PaintInternal(LayoutSurface layoutsurface, SKRect rect)
        {

            var mainsize = MainContents.Measure(layoutsurface.MeasureCache, rect.Size);

            // remaining
            var remainingrect = rect.WithHeight(rect.Height - mainsize.Height);
            if (remainingrect.Height > 0)
                layoutsurface.Paint(RemainingContents, remainingrect);

            // main
            var mainrect = rect.FromBottomHeight(mainsize.Height);
            layoutsurface.Paint(MainContents, mainrect);

            return rect;
        }
    }
}
