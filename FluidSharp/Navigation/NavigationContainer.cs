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
        public StatefulSlideTransition? SlideBackTransition;

        // current frame
        private IWidgetSource CurrentFrame => Stack.Peek();
        private IWidgetSource PreviousFrame { get { try { return Stack.Take(2).Last(); } catch { return Stack.Take(2).Last(); } } }


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
            OnStackChanged();
        }

        protected virtual void OnStackChanged() { }

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

                    OnStackChanged();

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

                OnStackChanged();

            }

            await Transition.Reverse();
            await OnNavigationStarted();

        }

        public virtual async Task OnTransitionCompleted(bool open)
        {
            if (open && TransitionTarget != null)
            {
                if (!Stack.Contains(TransitionTarget))
                {
                    Stack.Push(TransitionTarget);
                    OnStackChanged();
                }
            }
            Transition = null;
            TransitionTarget = null;
        }

        public virtual async Task OnSlideBackCompleted(VisualState visualState)
        {
            if (SlideBackState != null)
            {
                if (SlideBackState.Current == false)
                {
                    await visualState.EndEdit(false);
                    Stack.TryPop(out _);
                    SlideBackState = null;
                    OnStackChanged();
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
            var transitionTarget = TransitionTarget;
            var currentFrame = CurrentFrame;

            if (transition != null && transitionTarget != null)
            {
                if (!visualState.NavigationTop.IsContext(transitionTarget))
                    visualState.NavigationTop = new NavigationTop(transitionTarget);
                return transition.MakeWidget(this, visualState, currentFrame, transitionTarget);
            }

            if (Stack == null) throw new Exception("No root frame supplied");

            if (currentFrame is IPage page)
            {
                var canslideback = Stack.Count > 1 && page.CanSlideBack;
                if (canslideback)
                {
                    if (SlideBackState == null) SlideBackState = new SlideBackState(() => OnSlideBackCompleted(visualState));
                    if (SlideBackTransition is null || SlideBackTransition.State != SlideBackState) SlideBackTransition = new StatefulSlideTransition(visualState, SlideBackState, GetBackPanDetectorWidth(), MakeSlideBackWidget);
                    return SlideBackTransition;
                }

            }

            return currentFrame.MakeWidget(visualState);

        }

        public SKColor BackgroundColor
        {
            get
            {

                SKColor result = default;

                if (Stack.Count == 0) return default;

                var top = Stack.Peek();
                if (top is IBackgroundColorSource topBackgroundColorSource)
                {
                    result = topBackgroundColorSource.BackgroundColor;
                    if (result != default)
                        return result;
                }

                foreach (var item in Stack.ToArray())
                {
                    if (item is IBackgroundColorSource backgroundColorSource)
                    {
                        result = backgroundColorSource.BackgroundColor;
                        if (result != default)
                            return result;
                    }
                }
                return result;
            }
        }

    }
}
