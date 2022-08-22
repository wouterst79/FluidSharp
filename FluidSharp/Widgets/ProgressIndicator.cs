using FluidSharp.Layouts;
using FluidSharp.Paint;
using FluidSharp.State;
using FluidSharp.Widgets.CrossPlatform;
using SkiaSharp;
using SkiaSharp.TextBlocks.Enum;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace FluidSharp.Widgets
{
    public class ProgressIndicator : Widget
    {

        public float Height;
        public float LineHeight;

        public float ValuePct;

        public SKColor ValuePartColor;
        public SKColor OtherPartColor;

        public ProgressIndicator(float height, float lineheight, float valuepct, SKColor valuePartColor, SKColor otherPartColor)
        {
            Height = height;
            LineHeight = lineheight;
            ValuePct = valuepct;
            ValuePartColor = valuePartColor;
            OtherPartColor = otherPartColor;
        }

        public override SKSize Measure(MeasureCache measureCache, SKSize boundaries)
        {
            return new SKSize(boundaries.Width, Height);
        }

        public override SKRect PaintInternal(LayoutSurface layoutsurface, SKRect rect)
        {

            rect = rect.WithHeight(Height);

            var canvas = layoutsurface.Canvas;
            if (canvas != null)
            {

                var part = ValuePct;
                var w = rect.Width * (part);

                var isRtl = layoutsurface.IsRtl;
                var x1 = isRtl ? rect.Right : rect.Left;
                var x2 = isRtl ? rect.Right - w : rect.Left + w;
                var x3 = isRtl ? rect.Left : rect.Right;

                var halflineheight = LineHeight / 2;
                var liney = rect.Top + (rect.Height - LineHeight) / 2;

                // Value part
                if (part > 0)
                    canvas.DrawRoundRect(new SKRect(x1, liney, x2, liney + LineHeight), halflineheight, halflineheight, PaintCache.GetBackgroundPaint(ValuePartColor));

                // Other part
                if (part < 1)
                    canvas.DrawRoundRect(new SKRect(x2, liney, x3, liney + LineHeight), halflineheight, halflineheight, PaintCache.GetBackgroundPaint(OtherPartColor));

            }

            return rect;
        }

    }
}
