using FluidSharp.Animations;
using FluidSharp.Layouts;
using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace FluidSharp.Widgets
{
    public class HeightTransition : Animation.TimeBased
    {

        private HeightTransition(DateTime startTime, TimeSpan duration, float min, float delta, Widget child, Easing easing)
            : base(startTime, duration, min, delta, child, easing)
        {
        }

        public static Widget Make(DateTime appearingStarted, DateTime? disappearingStarted, TimeSpan duration, Widget child)
        {

            if (disappearingStarted.HasValue)
            {
                if (disappearingStarted.Value + duration < DateTime.Now)
                    return null;
                else
                    return new HeightTransition(disappearingStarted.Value, duration, 1, -1, child, Easing.CubicInOut);
            }
            else
            {
                if (appearingStarted + duration < DateTime.Now)
                    return child;
                else
                    return new HeightTransition(appearingStarted, duration, 0, 1, child, Easing.CubicInOut);
            }

        }

        public override SKSize Measure(MeasureCache measureCache, SKSize boundaries)
        {
            var childsize = Child.Measure(measureCache, boundaries);
            var pct = GetValue();
            return new SKSize(childsize.Width, childsize.Height * pct);
        }

        public override SKRect PaintInternal(LayoutSurface layoutsurface, SKRect rect)
        {

            var childsize = Child.Measure(layoutsurface.MeasureCache, rect.Size);

            var pct = GetValue();

            var height = childsize.Height * pct;
            var hidden = childsize.Height - height;

            //if (pct != 1)
              //  Debug.WriteLine($"height transition height: {height}%");

            var cliprect = new SKRect(rect.Left, rect.Top, rect.Right, rect.Top + height);


            // set clip rect
            layoutsurface.ClipRect(cliprect);

            // paint contents
            var childrect = new SKRect(rect.Left, rect.Top - hidden, rect.Right, rect.Top + height);
            layoutsurface.Paint(Child, childrect);

            // reset clip
            layoutsurface.ResetRectClip();

            return cliprect;

        }

    }
}
