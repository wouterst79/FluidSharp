using FluidSharp.State;
using FluidSharp.Widgets;
using System;
using System.Threading.Tasks;

namespace FluidSharp.Navigation
{
    public class NavigationTransitionState : TransitionState<bool>
    {
        public NavigationTransitionState(bool startingstate, Func<bool, Task>? onTransitionCompleted) : base(startingstate)
        {
            CurrentDuration = PushPageTransition.DefaultDuration;
            _ = SetTarget(!startingstate, default);
            if (onTransitionCompleted != null)
            {
                OnCompleted = () => onTransitionCompleted(Current);
            }
        }

        public override int GetDirection(bool from, bool to)
        {
            if (from == to) return 0;
            return from ? -1 : 1;
        }
    }
}
