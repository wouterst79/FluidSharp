using FluidSharp.Animations;
using FluidSharp.Layouts;
using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace FluidSharp.Widgets
{
    public class SlideInAnimation : AnimatedWidget
    {

        private SKSize ChildSize;

        public SlideInAnimation(Animation animation, Widget child)
            : base(animation, child)
        {
        }

        private SlideInAnimation(DateTime startTime, TimeSpan duration, float start, float end, Widget child)
            : base(new Animation(startTime, duration, start, end, Easing.CubicInOut), child)
        {
        }

        //public static Widget? Make(Animation animation, Widget child)
        //{

        //    var pct = animation.GetValue();

        //    if (pct <= 0)
        //        return null;
        //    else if (pct >= 1)
        //        return child;
        //    else
        //        return new SlideInAnimation(animation, child);

        //}

        public static Widget? Make(Animation? appearingStarted, Animation? disappearingStarted, Widget child)
        {

            if (disappearingStarted != null)
            {
                if (disappearingStarted.Completed)
                    return null;
                else
                    return new SlideInAnimation(disappearingStarted.StartTime, disappearingStarted.Duration, 1, 0, child);
            }
            else
            {
                if (appearingStarted == null || appearingStarted.Completed)
                    return child;
                else
                    return new SlideInAnimation(appearingStarted.StartTime, appearingStarted.Duration, 0, 1, child);
            }

        }

        public override SKSize Measure(MeasureCache measureCache, SKSize boundaries)
        {
            ChildSize = base.Measure(measureCache, boundaries);
            var pct = Animation.GetValue();
            return new SKSize(ChildSize.Width * pct, ChildSize.Height);
        }

        public override SKRect PaintInternal(LayoutSurface layoutsurface, SKRect rect)
        {


            //var delta = rect.Height * pct;

            //if (pct != 1)
            //  Debug.WriteLine($"height transition height: {height}%");

            //var cliprect = new SKRect(rect.Left, rect.Top + delta, rect.Right, rect.Top + rect.Height);

            if (Contents != null)
            {

                //var pct = 1 - Animation.GetValue();

                var childrect = layoutsurface.IsRtl ? new SKRect(rect.Right - ChildSize.Width, rect.Top, rect.Right, rect.Bottom) :
                                                      new SKRect(rect.Left, rect.Top, rect.Left + ChildSize.Width, rect.Bottom);

                // set clip rect
                //layoutsurface.ClipRect(cliprect);

                // paint contents
                //var childrect = new SKRect(rect.Left, rect.Top + delta, rect.Right, rect.Top + rect.Height + delta);
                layoutsurface.Paint(Contents, childrect);

                // reset clip
                //layoutsurface.ResetRectClip();

            }

            return rect;

        }

    }
}
