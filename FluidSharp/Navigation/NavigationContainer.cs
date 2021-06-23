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

        public virtual async Task Push(IWidgetSource Next)
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

        public virtual async Task Pop(IWidgetSource current)
        {

            var Transition = this.Transition;

            if (!Stack.Contains(current))
            {
                if (Transition == null)
                {
                    // assuming already popped / ignore
                    Debug.WriteLine($"unable to pop {current} - item not in navigation stack");
                    return;
                }
                else
                {
                    // assuming we're reversing an un-completed transition
                }
            }
            else
            {
                if (TransitionTarget != null && TransitionTarget != current)
                {
                    // trying to pop the item that's "the target of an existing pop" (IE double pop), or even lower on the stack
                    // - fast-forward poping

                    Transition = null;
                    while (CurrentFrame != current)
                        Stack.Pop();
                }
            }



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

                    this.Transition = Transition;

                }

                TransitionTarget = Stack.Pop();

            }

            await Transition.Reverse();
            await OnNavigationStarted();

        }

        public virtual async Task OnTransitionCompleted(bool open)
        {
            if (open && TransitionTarget != null)
            {
                Stack.Push(TransitionTarget);
            }
            Transition = null;
            TransitionTarget = null;
        }

        public async Task OnSlideBackCompleted(VisualState visualState)
        {
            if (SlideBackState != null)
            {
                if (SlideBackState.Current == false)
                {
                    await visualState.EndEdit(false);
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

        public virtual Widget MakeWidget(VisualState visualState)
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
                    if (SlideBackState == null) SlideBackState = new SlideBackState(() => OnSlideBackCompleted(visualState));
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
