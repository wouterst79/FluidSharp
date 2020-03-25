using SkiaSharp;
using SkiaSharp.TextBlocks.Enum;
using System;
using System.Collections.Generic;
using System.Text;

namespace FluidSharp.Widgets
{
    public class ShapeBidirectional : Shape
    {

        public SKPath LtrPath;
        public SKPath RtlPath;

        public ShapeBidirectional(SKPath ltrpath, SKPath rtlpath, SKColor backgroundColor, SKColor borderColor) : base(ltrpath, backgroundColor, borderColor)
        {
            LtrPath = ltrpath;
            RtlPath = rtlpath;
        }

        public override SKRect PaintInternal(LayoutSurface layoutsurface, SKRect rect)
        {
            if (layoutsurface.Device.FlowDirection == FlowDirection.LeftToRight)
                Path = LtrPath;
            else
                Path = RtlPath;
            return base.PaintInternal(layoutsurface, rect);
        }
    }
}
