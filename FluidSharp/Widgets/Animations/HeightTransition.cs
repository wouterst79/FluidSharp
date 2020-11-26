using FluidSharp.Animations;
using FluidSharp.Layouts;
using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace FluidSharp.Widgets
{
    public class HeightTransition : AnimatedWidget
    {

        public bool ScaleOnPaint { get; set; }

        private HeightTransition(DateTime startTime, TimeSpan duration, float start, float end, Widget child, bool scaleOnPaint)
            : base(new Animation(startTime, duration, start, end, Easing.CubicInOut), child)
        {
            ScaleOnPaint = scaleOnPaint;
        }

        public HeightTransition(Animation animation, Widget widget, bool scaleOnPaint) : base(animation, widget)
        {
            ScaleOnPaint = scaleOnPaint;
        }

        public static Widget? Make(DateTime appearingStarted, DateTime? disappearingStarted, TimeSpan duration, Widget child, bool scaleOnPaint)
        {

            if (disappearingStarted.HasValue)
            {
                if (disappearingStarted.Value + duration < DateTime.Now)
                    return null;
                else
                    return new HeightTransition(disappearingStarted.Value, duration, 1, 0, child, scaleOnPaint);
            }
            else
            {
                if (appearingStarted + duration < DateTime.Now)
                    return child;
                else
                    return new HeightTransition(appearingStarted, duration, 0, 1, child, scaleOnPaint);
            }

        }

        public override SKSize Measure(MeasureCache measureCache, SKSize boundaries)
        {
            var childsize = base.Measure(measureCache, boundaries);
            var pct = Animation.GetValue();
            return new SKSize(childsize.Width, childsize.Height * pct);
        }

        public override SKRect PaintInternal(LayoutSurface layoutsurface, SKRect rect)
        {

            var childsize = base.Measure(layoutsurface.MeasureCache, rect.Size);

            var pct = ScaleOnPaint ? Animation.GetValue() : 1;

            var height = childsize.Height * pct;
            var hidden = 0;// childsize.Height - height;

            var cliprect = new SKRect(rect.Left, rect.Top, rect.Right, rect.Top + height);

            if (Child != null)
            {

                //if (pct != 1)
                //  Debug.WriteLine($"height transition height: {height}%");


                // set clip rect
                layoutsurface.ClipRect(cliprect);

                // paint contents
                var childrect = new SKRect(rect.Left, rect.Top - hidden, rect.Right, rect.Top + height);
                layoutsurface.Paint(Child, childrect);

                // reset clip
                layoutsurface.ResetRectClip();

            }
            return cliprect;

        }

    }
}
