using SkiaSharp;
using SkiaSharp.TextBlocks.Enum;
using System;
using System.Collections.Generic;
using System.Text;

namespace FluidSharp.Layouts
{
    public struct Margins
    {

        public float Near;
        public float Top;
        public float Far;
        public float Bottom;

        public float TotalX => Near + Far;
        public float TotalY => Top + Bottom;

        public Margins(float near, float top, float far, float bottom)
        {
            Near = near;
            Top = top;
            Far = far;
            Bottom = bottom;
        }

        public Margins(float all)
        {
            Near = all;
            Top = all;
            Far = all;
            Bottom = all;
        }

        public Margins(float x, float y)
        {
            Near = x;
            Top = y;
            Far = x;
            Bottom = y;
        }

        public SKRect Apply(SKRect original, FlowDirection flowDirection)
        {
            if (flowDirection == FlowDirection.LeftToRight)
                return new SKRect(original.Left + Near, original.Top + Top, original.Right - Far, original.Bottom - Bottom);
            else
                return new SKRect(original.Left + Far, original.Top + Top, original.Right - Near, original.Bottom - Bottom);
        }

    }
}
