using FluidSharp.Layouts;
using FluidSharp.Widgets;
using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Text;

namespace FluidSharp.Animations
{
    public class AlignAnimation : Widget
    {

        public Align A;
        public Align B;

        public float Percentage;

        public AlignAnimation(Align a, Align b, float percentage)
        {
            A = a ?? throw new ArgumentNullException(nameof(a));
            B = b ?? throw new ArgumentNullException(nameof(b));
            Percentage = percentage;
        }

        public static Widget Make(Align a, Align b, float pct)
        {
            if (pct <= 0) return a;
            if (pct >= 1) return b;
            return new AlignAnimation(a, b, pct);
        }

        public override SKSize Measure(MeasureCache measureCache, SKSize boundaries)
        {
            if (A.Child == B.Child) return A.Measure(measureCache, boundaries);
            var sa = A.Measure(measureCache, boundaries);
            var sb = B.Measure(measureCache, boundaries);
            var pcta = (1 - Percentage);
            var pctb = Percentage;
            return new SKSize(sa.Width * pcta + sb.Width * pctb, sa.Height * pcta + sb.Height * pctb);
        }

        public override SKRect PaintInternal(LayoutSurface layoutsurface, SKRect rect)
        {
            var recta = A.GetChildRect(layoutsurface, rect);
            var rectb = B.GetChildRect(layoutsurface, rect);

            var pcta = (1 - Percentage);
            var pctb = Percentage;

            var childrect = new SKRect(recta.Left * pcta + rectb.Left * pctb,
                                       recta.Top * pcta + rectb.Top * pctb,
                                       recta.Right * pcta + rectb.Right * pctb,
                                       recta.Bottom * pcta + rectb.Bottom * pctb);

            layoutsurface.Paint(A.Child, childrect);

            return childrect;
        }

    }
}
