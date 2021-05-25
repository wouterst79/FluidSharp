using FluidSharp.Layouts;
using FluidSharp.State;
using FluidSharp.Widgets;
using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FluidSharp.Navigation
{

    public class SlideBackState : CarouselState<bool>
    {
        public SlideBackState(Func<Task> onSlideBackCompleted) : base(true)
        {
            OnCompleted = onSlideBackCompleted;
        }

        public override int GetDirection(bool from, bool to)
        {
            if (from == to) return 0;
            return from ? -1 : 1;
        }

        public override bool GetNextValue(bool from, int direction)
        {
            return !from;
        }

    }

    public abstract class NavigationContainer : IWidgetSource, IBackgroundColorSource
    {

        public Stack<IWidgetSource> Stack = new Stack<IWidgetSource>();

        public Func<Task> OnNavigationStarted;

        // transition
        public IPageTransition? Transition;
        public IWidgetSource? TransitionTarget;

        // slide back
        public CarouselState<bool>? SlideBackState;

        // current frame
        private IWidgetSource CurrentFrame => Stack.Peek();
        private IWidgetSource PreviousFrame => Stack.Take(2).Last();


        protected abstract float GetBackPanDetectorWidth();

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

            Transition = null;
            if (Transition is null) // not sure why this was here, but it breaks multi-popup (choose meals from meal details)
            {
                if (Next is IPage page)
                    Transition = page.GetPageTransition(false, OnTransitionCompleted);
                if (Transition is null)
                    Transition = new PushPageTransition(false, OnTransitionCompleted);
            }

            TransitionTarget = Next;

            await Transition.Start();
            await OnNavigationStarted();

        }

        public async Task Pop(IWidgetSource current)
        {

            if (CurrentFrame != current)
                throw new Exception();

            if (Transition == null)
            {

                if (Stack.Count <= 1)
                    return;
                //throw new Exception("nothing to pop");

                if (Transition is null)
                {
                    if (CurrentFrame is IPage page)
                        Transition = page.GetPageTransition(true, OnTransitionCompleted);

                    if (PreviousFrame is IPage previous)
                    {
                        var poptask = previous.PrepareForResurface();
                        if (poptask != null) await poptask();
                    }

                    if (Transition is null)
                        Transition = new PushPageTransition(true, OnTransitionCompleted);

                }

                TransitionTarget = Stack.Pop();

            }

            await Transition.Reverse();
            await OnNavigationStarted();

        }

        public async Task OnTransitionCompleted(bool open)
        {
            if (open && TransitionTarget != null)
            {
                Stack.Push(TransitionTarget);
            }
            Transition = null;
            TransitionTarget = null;
        }

        public async Task OnSlideBackCompleted()
        {
            if (SlideBackState != null)
            {
                if (SlideBackState.Current == false)
                {
                    Stack.Pop();
                    SlideBackState = null;
                }
            }
        }

        public Widget MakeSlideBackWidget(VisualState visualState, bool istarget)
        {
            if (istarget)
                return CurrentFrame.MakeWidget(visualState);
            else
                return PreviousFrame.MakeWidget(visualState);
        }

        public Widget MakeWidget(VisualState visualState)
        {

            var transition = Transition;
            if (transition != null)
            {
                if (!visualState.NavigationTop.IsContext(TransitionTarget))
                    visualState.NavigationTop = new NavigationTop(TransitionTarget!);
                return transition.MakeWidget(this, visualState, CurrentFrame, TransitionTarget!);
            }

            if (Stack == null) throw new Exception("No root frame supplied");

            var currentframe = CurrentFrame;

            if (currentframe is IPage page)
            {
                var canslideback = Stack.Count > 1 && page.CanSlideBack;
                if (canslideback)
                {
                    if (SlideBackState == null) SlideBackState = new SlideBackState(OnSlideBackCompleted);
                    return SlideBackNavigation.Make(visualState, SlideBackState, SlideBackState.GetFrame(), GetBackPanDetectorWidth(), 0, null, MakeSlideBackWidget);
                }

            }

            return currentframe.MakeWidget(visualState);

        }

        public SKColor GetBackgroundColor(VisualState visualState)
        {
            SKColor result = default;
            foreach (var item in Stack)
            {
                if (item is IBackgroundColorSource backgroundColorSource)
                {
                    result = backgroundColorSource.GetBackgroundColor(visualState);
                    if (result != default)
                        return result;
                }
            }
            return result;
        }
    }
}
