using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Text;

namespace FluidSharp
{
    public static class SKPaintExtensions
    {

        public static SKColor WithOpacity(this SKColor color, float opacity) => opacity == 1 ? color : color.WithAlpha((byte)(255 * opacity));

    }
}
