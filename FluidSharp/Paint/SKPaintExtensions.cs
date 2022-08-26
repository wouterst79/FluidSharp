using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Text;

namespace FluidSharp
{

    //public class SKPaint
    //{
    //    private SKPaint() { }
    //}

    public static class SKPaintExtensions
    {

        public static SKColor MixColor(this SKColor color1, SKColor color2, float twopct)
        {
            if (twopct <= 0) return color1;
            if (twopct >= 1) return color2;
            var onepct = 1 - twopct;
            return new SKColor(Mix(color1.Red, color2.Red), Mix(color1.Green, color2.Green), Mix(color1.Blue, color2.Blue), Mix(color1.Alpha, color2.Alpha));
            byte Mix(byte c1, byte c2) => (byte)(c1 * onepct + c2 * twopct);
        }

        public static SKColor WithOpacity(this SKColor color, float opacity) => opacity >= 1 ? color : color.WithAlpha((byte)(opacity * color.Alpha));

        public static bool IsLight(this SKColor color)
        {
            color.ToHsl(out var h, out var s, out var l);
            return l > 60f;
        }
    }
}
