#if false
using FluidSharp.Animations;
using FluidSharp.State;
using FluidSharp.Widgets;
using FluidSharp.Widgets.Animations;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace FluidSharp.Navigation
{
    public class HeroPageTransition : IPageTransition
    {

        public NavigationTransitionState TransitionState;
        public Func<bool, Task> OnTransitionCompleted;

        public Task Start() => TransitionState.SetTarget(true, null);
        public Task Reverse() => TransitionState.SetTarget(false, null);

        public HeroPageTransition(bool start, Func<bool, Task> onTransitionCompleted)
        {
            OnTransitionCompleted = onTransitionCompleted;
            TransitionState = new NavigationTransitionState(start, OnTransitionCompleted);
            TransitionState.TransitionDuration = TimeSpan.FromSeconds(1);
        }

        public Widget MakeWidget(VisualState visualState, IWidgetSource from, IWidgetSource to, Func<Task> dismiss)
        {
            var frame = TransitionState.GetFrame();

            var transition = HeroTransition.MakeWidget(visualState, frame, (vs, istarget) => !istarget ? from.MakeWidget(vs) : to.MakeWidget(vs));
            //var transition = SlideTransition.MakeWidget(visualState, frame, 0.5f, (vs, istarget) => !istarget ? from.MakeWidget(vs) : to.MakeWidget(vs));

            return transition;
        }


    }
}
#endif