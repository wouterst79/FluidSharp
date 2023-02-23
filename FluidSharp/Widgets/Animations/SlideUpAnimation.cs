using FluidSharp.Animations;
using FluidSharp.Layouts;
using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace FluidSharp.Widgets
{
    public class SlideUpAnimation : AnimatedWidget
    {

        private SlideUpAnimation(DateTime startTime, TimeSpan duration, float start, float end, Widget child)
            : base(new Animation(startTime, duration, start, end, Easing.CubicInOut), child)
        {
        }

        public static Widget? Make(Animation? appearingStarted, Animation? disappearingStarted, Widget child)
        {

            if (disappearingStarted != null)
            {
                if (disappearingStarted.Completed)
                    return null;
                else
                    return new SlideUpAnimation(disappearingStarted.StartTime, disappearingStarted.Duration, 1, 0, child);
            }
            else
            {
                if (appearingStarted == null || appearingStarted.Completed)
                    return child;
                else
                    return new SlideUpAnimation(appearingStarted.StartTime, appearingStarted.Duration, 0, 1, child);
            }

        }

        public static Widget? Make(DateTime appearingStarted, DateTime? disappearingStarted, TimeSpan duration, Widget child)
        {

            if (disappearingStarted.HasValue)
            {
                if (disappearingStarted.Value + duration < DateTime.Now)
                    return null;
                else
                    return new SlideUpAnimation(disappearingStarted.Value, duration, 1, 0, child);
            }
            else
            {
                if (appearingStarted + duration < DateTime.Now)
                    return child;
                else
                    return new SlideUpAnimation(appearingStarted, duration, 0, 1, child);
            }

        }

        public override SKSize Measure(MeasureCache measureCache, SKSize boundaries) => base.Measure(measureCache, boundaries);

        public override SKRect PaintInternal(LayoutSurface layoutsurface, SKRect rect)
        {

            //var childsize = Child.Measure(layoutsurface.MeasureCache, rect.Size);

            var pct = 1 - Animation.GetValue();

            var delta = rect.Height * pct;

            //if (pct != 1)
            //  Debug.WriteLine($"height transition height: {height}%");

            var cliprect = new SKRect(rect.Left, rect.Top + delta, rect.Right, rect.Top + rect.Height);

            if (Contents != null)
            {


                // set clip rect
                layoutsurface.ClipRect(cliprect);

                // paint contents
                var childrect = new SKRect(rect.Left, rect.Top + delta, rect.Right, rect.Top + rect.Height + delta);
                layoutsurface.Paint(Contents, childrect);

                // reset clip
                layoutsurface.ResetRectClip();

            }

            return cliprect;

        }

    }
}
