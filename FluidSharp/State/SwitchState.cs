using FluidSharp.Animations;
using FluidSharp.Engine;
using FluidSharp.Widgets;
using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using static FluidSharp.Widgets.Scrollable;

namespace FluidSharp.State
{
    public class SwitchState : TransitionState<bool>
    {

        public DateTime? LastSwitched;

        public SwitchState(bool startingstate, Func<bool, Task> onSettingChanged) : base(startingstate)
        {
            TransitionDuration = TimeSpan.FromMilliseconds(125);// SlideTransition.DefaultDuration;
            OnCompleted = () => onSettingChanged(Current);
        }

        public override int GetDirection(bool from, bool to)
        {
            if (from == to) return 0;
            return from ? -1 : 1;
        }

        public override Animation GetAnimation(Easing easing)
        {

            var current = Current;
            var target = Target;

            var min = target ? 0 : 1;
            var delta = target ? 1 : -1;

            if (current == target)
                return new Animation(AnimationStart, CurrentDuration, min, min + delta, easing);
            else
                return new Animation(AnimationStart, CurrentDuration, min, min + delta, easing, () => Progress(GetFrame(), null));

        }

        public Task Toggle() => SetTarget(!Target, null);



    }
}
