#if DEBUG
#define SHOWSPACING
#endif
using FluidSharp.Layouts;
using SkiaSharp;
using SkiaSharp.TextBlocks.Enum;
using System;
using System.Collections.Generic;
using System.Text;

namespace FluidSharp.Widgets
{
    public class Spacing : Widget
    {

        public SKSize Size;

        private static Dictionary<float, Spacing> SquareCache = new Dictionary<float, Spacing>();
        private static Dictionary<(float width, float height), Spacing> RectCache = new Dictionary<(float width, float height), Spacing>();

        private Spacing(float size) => Size = new SKSize(size, size);
        private Spacing(float width, float height) => Size = new SKSize(width, height);

        public static Spacing Make(float size)
        {
            if (!SquareCache.TryGetValue(size, out var result)) SquareCache[size] = result = new Spacing(size);
            return result;
        }

        public static Spacing Make(float width, float height)
        {
            if (!RectCache.TryGetValue((width, height), out var result)) RectCache[(width, height)] = result = new Spacing(width, height);
            return result;
        }

        public override SKSize Measure(MeasureCache measureCache, SKSize boundaries)
        {
            return Size;
        }

        public override SKRect PaintInternal(LayoutSurface layoutsurface, SKRect rect)
        {

            var x = rect.Left;
            if (layoutsurface.Device.FlowDirection == FlowDirection.RightToLeft)
                x = rect.Right - Size.Width;

            var y = rect.Top;
            var dest = new SKRect(x, y, x + Size.Width, y + Size.Height);

#if SHOWSPACING
            layoutsurface.DebugSpacing(dest, () => $"{Size.Width}x{Size.Height}", SKColors.Blue);
#endif

            return dest;
        }
    }
}
