using FluidSharp.Widgets;
using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Text;

namespace FluidSharp.Layouts
{
    public static class SKRectExtensions
    {

        public static SKRect Scale(this SKRect rect, ScaleMode scaleMode, SKSize aspect)
        {

            if (scaleMode == ScaleMode.Strech) return rect;

            var w = rect.Width;
            var h = rect.Height;

            var wfromh = h * aspect.Width / aspect.Height;
            var hfromw = w * aspect.Height / aspect.Width;

            if (scaleMode == ScaleMode.Fit && wfromh < w || scaleMode == ScaleMode.Fill && wfromh > w)
                return new SKRect(rect.MidX - wfromh / 2, rect.Top, rect.MidX + wfromh / 2, rect.Bottom);
            else
                return new SKRect(rect.Left, rect.MidY - hfromw / 2, rect.Right, rect.MidY + hfromw / 2);
        }

    }
}
