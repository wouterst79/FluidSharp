using FluidSharp.State;
using FluidSharp.Widgets;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace FluidSharp.Navigation
{
    public class PushPageTransition : IPageTransition
    {

        public const float PushOverlap = .5f;

        public NavigationTransitionState TransitionState;
        public Func<bool, Task> OnTransitionCompleted;

        public static TimeSpan DefaultDuration = TimeSpan.FromMilliseconds(250);

        public Task Start() => TransitionState.SetTarget(true, null);
        public Task Reverse() => TransitionState.SetTarget(false, null);

        public PushPageTransition(bool start, Func<bool, Task> onTransitionCompleted)
        {
            OnTransitionCompleted = onTransitionCompleted;
            TransitionState = new NavigationTransitionState(start, OnTransitionCompleted);
        }

        public Widget MakeWidget(VisualState visualState, IWidgetSource from, IWidgetSource to, Func<Task> dismiss)
        {
            var frame = TransitionState.GetFrame();
            return SlideTransition.MakeWidget(visualState, frame, PushOverlap, (vs, istarget) => !istarget ? from.MakeWidget(vs) : to.MakeWidget(vs));
        }


    }
}
