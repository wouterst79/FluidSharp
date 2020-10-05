#if DEBUG
//#define DEBUGSETTARGET
#endif
using FluidSharp.Animations;
using FluidSharp.Widgets;
using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace FluidSharp.State
{

    public abstract class TransitionState<T>
    {

        public T Current { get; protected set; }
        public T Target { get; protected set; }
        public DateTime AnimationStart;

        public TimeSpan TransitionDuration;

        public Func<Task>? OnCompleted;

        public const int MillisecondsForFinishedAnimation = -50000;

        public TransitionState(T InitialValue)
        {
            Current = Target = InitialValue;
            SetAnimationStart(MillisecondsForFinishedAnimation);
        }

        public abstract int GetDirection(T from, T to);


        public TransitionFrame<T> GetFrame()
        {
            var frame = GetFrameRatios();

            var current = Current;

            var ratio = frame.ratio;
            var direction = frame.direction;

            var next = Target;

            return new TransitionFrame<T>(ratio, current, direction, next, Progress);
        }

        public Animation GetAnimation(Easing easing)
        {

            var current = Current;
            var target = Target;

            var direction = GetDirection(current, target);
            var min = direction >= 0 ? 0 : 1;
            var delta = direction >= 0 ? 1 : -1;

            return new Animation(AnimationStart, TransitionDuration, min, min + delta, easing, () => Progress(GetFrame(), null));

        }

        private (float ratio, int direction) GetFrameRatios()
        {
            var current = Current;
            var target = Target;
            var started = AnimationStart;

            var ratio = (float)(DateTime.Now.Subtract(started).TotalMilliseconds / TransitionDuration.TotalMilliseconds);
            if (ratio > 1) ratio = 1;

            var direction = GetDirection(current, target);
            if (direction == 0) ratio = 0;

            return (ratio, direction);

        }

        public virtual async Task SetTarget(T target, VisualState? visualState, float velocity = 1f)
        {

            var currentdirection = GetDirection(Current, Target);
            var targetdirection = GetDirection(Current, target);

            var newduration = TimeSpan.FromMilliseconds(SlideTransition.DefaultDuration.TotalMilliseconds / velocity);

            if (currentdirection == 0)
            {
                TransitionDuration = newduration;
                SetAnimationStart(0);
            }
            else if (currentdirection != targetdirection)
            {

#if DEBUGSETTARGET
                System.Diagnostics.Debug.WriteLine($"direction changed {Current} > {Target} ({currentdirection})     {Current} > {target} ({targetdirection})");
#endif
                // changing direction on an in-progress animation
                var frame = GetFrame();
                var ratio = frame.Ratio;
                TransitionDuration = newduration;
                Current = Target;
                var completed = (ratio - 1) * TransitionDuration.TotalMilliseconds;
                SetAnimationStart(completed);
            }
            else
            {
                TransitionDuration = newduration;
            }

#if DEBUGSETTARGET
            System.Diagnostics.Debug.WriteLine($"target set to {Current} > {target}   from {Current} > {Target}  ({currentdirection} , {targetdirection})");
#endif

            Target = target;
            if (visualState != null)
                await visualState.RequestRedraw();

        }

        protected void SetAnimationStart(double millisecondsfromnow)
        {
            AnimationStart = DateTime.Now.AddMilliseconds(millisecondsfromnow);
        }

        public async Task Progress(TransitionFrame<T> frame, VisualState visualState)
        {
            Current = frame.Next;
            if (OnCompleted != null)
                await OnCompleted();
        }

    }
}
