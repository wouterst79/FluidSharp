using FluidSharp.Animations;
using FluidSharp.Layouts;
using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace FluidSharp.Widgets
{
    public abstract class Animation : Widget
    {

        public Widget Child;

        private bool completed;
        public bool Completed { get => completed; set { completed = value; if (value && OnCompleted != null) Task.Run(() => OnCompleted()); } }
        public Func<Task> OnCompleted;

        public static TimeSpan DefaultAnimationDuration = TimeSpan.FromMilliseconds(250);


        protected Animation()
        {
        }

        public override SKSize Measure(MeasureCache measureCache, SKSize boundaries) => Child.Measure(measureCache, boundaries);
        public override SKRect PaintInternal(LayoutSurface layoutsurface, SKRect rect) => layoutsurface.Paint(Child, rect);

        public abstract float GetValue();

        public class TimeBased : Animation
        {

            public DateTime StartTime;
            public TimeSpan Duration;
            public Easing Easing;

            public float Min;
            public float Delta;

            public TimeBased(DateTime startTime, TimeSpan duration, float min, float delta, Widget child, Easing easing)
            {
                StartTime = startTime;
                Duration = duration;
                Min = min;
                Delta = delta;
                Child = child;
                Easing = easing;
            }

            public TimeBased(DateTime startTime, TimeSpan duration, float min, float delta, Func<Task> onCompleted)
            {
                StartTime = startTime;
                Duration = duration;
                Min = min;
                Delta = delta;
                OnCompleted = onCompleted;
            }

            public TimeBased(DateTime startTime, TimeSpan duration, float min, float delta, Func<float, Widget> makechild)
            {
                StartTime = startTime;
                Duration = duration;
                Min = min;
                Delta = delta;
                Child = makechild(GetValue());
            }

            public TimeBased(DateTime startTime, TimeSpan duration, float min, float delta, Func<float, Widget> makechild, Func<Task> onCompleted)
            {
                StartTime = startTime;
                Duration = duration;
                Min = min;
                Delta = delta;
                Child = makechild(GetValue());
                OnCompleted = onCompleted;
            }

            public override float GetValue()
            {
                var delta = DateTime.Now.Subtract(StartTime).TotalMilliseconds / Duration.TotalMilliseconds;
                if (delta < 0) delta = 0;
                if (delta >= 1) { delta = 1; Completed = true; }

                var value = (float)(Min + delta * Delta);
                if (Easing != null) value = Easing.Ease(value);
                return value;
            }

        }

    }
}
