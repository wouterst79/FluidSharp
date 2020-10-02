using FluidSharp.Layouts;
using FluidSharp.State;
using FluidSharp.Widgets;
using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace FluidSharp.Navigation
{

    public class NavigationTransitionState : TransitionState<bool>
    {
        public NavigationTransitionState(bool startingstate, Func<Task> onTransitionCompleted) : base(startingstate)
        {
            TransitionDuration = HeroTransition.DefaultDuration;
            OnCompleted = onTransitionCompleted;
        }

        public override int GetDirection(bool from, bool to)
        {
            if (from == to) return 0;
            return from ? -1 : 1;
        }
    }

    public class NavigationContainer : IWidgetSource
    {

        public Stack<IWidgetSource> Stack = new Stack<IWidgetSource>();

        public Func<Task> OnNavigationStarted;

        public NavigationTransitionState? TransitionState;
        public IWidgetSource? TransitionTarget;

        private IWidgetSource CurrentFrame => Stack.Peek();

        public NavigationContainer(IWidgetSource rootframe, Func<Task> onnavigationstarted)
        {
            SetRootFrame(rootframe);
            OnNavigationStarted = onnavigationstarted;
        }

        protected NavigationContainer(Func<Task> onnavigationstarted)
        {
            OnNavigationStarted = onnavigationstarted;
        }

        protected void SetRootFrame(IWidgetSource rootframe)
        {
            Stack = new Stack<IWidgetSource>();
            Stack.Push(rootframe);
        }

        public async Task Push(IWidgetSource Next)
        {


            if (TransitionState == null)
                TransitionState = new NavigationTransitionState(false, OnTransitionCompleted);
            TransitionTarget = Next;

            await TransitionState.SetTarget(true, null);
            await OnNavigationStarted();

        }

        public async Task Pop()
        {

            if (Stack.Count <= 1)
                throw new Exception("nothing to pop");

            if (TransitionState == null)
                TransitionState = new NavigationTransitionState(true, OnTransitionCompleted);

            TransitionTarget = Stack.Pop();

            await TransitionState.SetTarget(false, null);
            await OnNavigationStarted();

        }

        public async Task OnTransitionCompleted()
        {
            if (TransitionState != null && TransitionTarget != null)
            {
                if (TransitionState.Current == true)
                    Stack.Push(TransitionTarget);
            }
            TransitionState = null;
            TransitionTarget = null;
        }

        public Widget MakeTransitionWidget(VisualState visualState, bool istarget)
        {
            if (istarget)
                return TransitionTarget!.MakeWidget(visualState);
            else
                return CurrentFrame.MakeWidget(visualState);
        }

        public Widget MakeWidget(VisualState visualState)
        {

            if (TransitionState != null)
            {
                var frame = TransitionState.GetFrame();
                return SlideTransition.MakeWidget(visualState, frame, MakeTransitionWidget);
            }

            if (Stack == null) throw new Exception("No root frame supplied");

            return CurrentFrame.MakeWidget(visualState);

        }

    }
}
