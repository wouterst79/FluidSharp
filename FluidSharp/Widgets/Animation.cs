using FluidSharp.Animations;
using FluidSharp.Layouts;
using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Diagnostics;
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

        public override SKSize Measure(MeasureCache measureCache, SKSize boundaries) => Child?.Measure(measureCache, boundaries) ?? new SKSize();
        public override SKRect PaintInternal(LayoutSurface layoutsurface, SKRect rect) => Child == null ? rect : layoutsurface.Paint(Child, rect);

        public abstract float GetValue();


        public static void TestStagger()
        {


            for (var c = 2; c < 20; c++)
            {

                Debug.Assert(Stagger(0, c, c / 3 + 1, 0) == 0);
                Debug.Assert(Stagger(1, c, c / 3 + 1, 0) == 0);

                Debug.Assert(Stagger(0, c, c / 3 + 1, 1) == 1);
                Debug.Assert(Stagger(1, c, c / 3 + 1, 1) > .999f);
                Debug.Assert(Stagger(c - 1, c, c / 3 + 1, 1) > .999f);

            }
        }

        public static float Stagger(int part, int partcount, int overlappingpartcount, float pct)
        {

            if (overlappingpartcount < 0) throw new ArgumentOutOfRangeException(nameof(overlappingpartcount));
            if (part > partcount - 1) throw new ArgumentOutOfRangeException(nameof(part));

            // 0               1
            // partcount: 2, overlappingpartcount: 1
            // --------- 
            //          --------

            // partcount: 2, overlappingpartcount: 2
            // ----------
            //      ------------

            // partcount: 2, overlappingpartcount: 3
            // -----------
            //     -------------

            // partcount: 3, overlappingpartcount: 2
            // --------
            //      --------
            //          --------


            // partcount: 7, overlappingpartcount: 3
            // ------
            //   ------
            //     ------
            //       ------
            //         ------
            //           ------
            //             ------

            var totalsegments = 1f + ((float)(partcount - 1)) / overlappingpartcount;
            var segmentlength = 1f / totalsegments;
            var delta = segmentlength / overlappingpartcount;

            var position = (pct - (delta * part)) / segmentlength;
            if (position < 0) return 0;
            if (position > 1) return 1;
            return position;

        }

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
