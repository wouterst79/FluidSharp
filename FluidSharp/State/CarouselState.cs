#if DEBUG
//#define DEBUGSETTARGET
#endif
using FluidSharp.Widgets;
using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace FluidSharp.State
{

    public class TransitionFrame<T>
    {

        public float Ratio;

        public T Current;
        public int Direction;
        public T Next;

        public Func<TransitionFrame<T>, VisualState, Task>? OnCompleted;

        public TransitionFrame(float ratio, T current, int direction, T next, Func<TransitionFrame<T>, VisualState, Task>? onCompleted)
        {
            Ratio = ratio;
            Current = current;
            Direction = direction;
            Next = next;
            OnCompleted = onCompleted;
        }

    }

    public abstract class CarouselState<T>
    {

        public T Current { get; protected set; }
        public T Target { get; protected set; }
        public DateTime AnimationStart;

        public float? PanRatio;
        public bool IsReverse;


        public TimeSpan TransitionDuration;


        public Func<Task>? OnCompleted;

        public const float SnapThreshold = .6f;
        public const float FlingingVelocity = 200; // pixels per seconds
        public const int MillisecondsForFinishedAnimation = -50000;

        public CarouselState(T InitialValue)
        {
            Current = Target = InitialValue;
            SetAnimationStart(MillisecondsForFinishedAnimation, false);
        }

        public abstract int GetDirection(T from, T to);

        public abstract T GetNextValue(T from, int direction);


        public bool IsAnimating => GetDirection(Current, Target) != 0;


        public TransitionFrame<T> GetFrame()
        {
            var frame = GetFrameRatios();

            var current = Current;

            var ratio = frame.ratio;
            var direction = frame.direction;

            var next = direction == 0 ? default : GetNextValue(current, direction);

            while (ratio > 1)
            {
                ratio -= 1;
                current = next;
                next = GetNextValue(current, direction);
            }

            return new TransitionFrame<T>(ratio, current, direction, next, Progress);
        }

        private (float ratio, int direction) GetFrameRatios()
        {
            var current = Current;
            if (PanRatio.HasValue)
                if (PanRatio > 0)
                    return (PanRatio.Value, -1);
                else
                    return (-PanRatio.Value, 1);
            else
            {

                var target = Target;
                var started = AnimationStart;

                var ratio = (float)(DateTime.Now.Subtract(started).TotalMilliseconds / TransitionDuration.TotalMilliseconds);
                if (ratio > 1) ratio = 1;

                var direction = GetDirection(current, target);
                if (direction == 0) ratio = 0;

                return (ratio, direction);
            }

        }

        public virtual async Task SetTarget(T target, VisualState visualState, float velocity)
        {

            var currentdirection = GetDirection(Current, Target);
            var targetdirection = GetDirection(Current, target);

            PanRatio = null;

            var newduration = TimeSpan.FromMilliseconds(SlideTransition.DefaultDuration.TotalMilliseconds / velocity);

            if (currentdirection == 0)
            {
                TransitionDuration = newduration;
                SetAnimationStart(0, false);
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
                Current = GetNextValue(Current, currentdirection);
                var completed = (ratio - 1) * TransitionDuration.TotalMilliseconds;
                SetAnimationStart(completed, false);
            }
            else
            {
                TransitionDuration = newduration;
            }

#if DEBUGSETTARGET
            System.Diagnostics.Debug.WriteLine($"target set to {Current} > {target}   from {Current} > {Target}  ({currentdirection} , {targetdirection})");
#endif

            Target = target;
            await visualState.RequestRedraw();

        }

        protected void SetAnimationStart(double millisecondsfromnow, bool isReverse)
        {
            AnimationStart = DateTime.Now.AddMilliseconds(millisecondsfromnow);
            IsReverse = isReverse;
        }

        public async Task SetRelativePan(float pan, VisualState visualState)
        {

            if (!PanRatio.HasValue)
                if (IsAnimating)
                    // jump to the last frame 
                    Current = Target;

            PanRatio = pan;

            await visualState.RequestRedraw();

        }

        public async Task EndPan(SKPoint velocity, VisualState visualState)
        {

            TransitionDuration = SlideTransition.DefaultDuration;

            // start "snap" transition
            var frame = GetFrame();
            var ratio = frame.Ratio;
            var flinging = Math.Abs(velocity.X) > FlingingVelocity;

            if (ratio == 0)
            {
                Current = Target = frame.Current;
                SetAnimationStart(MillisecondsForFinishedAnimation, false);
            }
            else if (flinging || ratio > SnapThreshold)
            {
                // proceed
                Current = frame.Current;
                Target = frame.Next;
                var completed = -ratio * TransitionDuration.TotalMilliseconds;
                SetAnimationStart(completed, false);
            }
            else
            {
                // revert
                Current = frame.Next;
                Target = frame.Current;
                var completed = (ratio - 1) * TransitionDuration.TotalMilliseconds;
                SetAnimationStart(completed, true);
            }

            PanRatio = null;

            await visualState.RequestRedraw();

        }

        private async Task Progress(TransitionFrame<T> frame, VisualState visualState)
        {

            var NextStep = frame.Next;
            if (NextStep == null) return;
            if (GetDirection(Current, Target) == GetDirection(Current, NextStep)) // make sure the transition is still valid
            {

                //System.Diagnostics.Debug.WriteLine($"current StateTransition set to {NextStep} from {Current} (target = {Target})");

                Current = NextStep;
                SetAnimationStart(0, false);

                await visualState.RequestRedraw();

                if (OnCompleted != null)
                    await OnCompleted();
                
            }

        }

    }
}
