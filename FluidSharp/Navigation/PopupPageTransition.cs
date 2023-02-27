using FluidSharp.Animations;
using FluidSharp.Layouts;
using FluidSharp.State;
using FluidSharp.Touch;
using FluidSharp.Widgets;
using FluidSharp.Widgets.Animations;
using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace FluidSharp.Navigation
{
    public class PopupPageTransition : IPageTransition
    {

        public SKColor MaskColor;
        public NavigationTransitionState TransitionState;
        private Func<bool, Task> OnTransitionCompleted2;
        public bool UseHeightTransition = true;

        public Task Start() => TransitionState.SetTarget(true, null);
        public Task Reverse() => TransitionState.SetTarget(false, null);

        public PopupPageTransition(bool startingstate, SKColor maskColor, Func<bool, Task> onTransitionCompleted)
        {
            MaskColor = maskColor;
            OnTransitionCompleted2 = onTransitionCompleted;
            TransitionState = new NavigationTransitionState(startingstate, OnTransitionCompleted);
            //TransitionState.StandardDuration = TimeSpan.FromSeconds(2);
        }

        public Task OnTransitionCompleted(bool open) => !open ? OnTransitionCompleted2(open) : Task.CompletedTask;

        public Widget MakeWidget(NavigationContainer navigationContainer, VisualState visualState, IWidgetSource from, IWidgetSource to)
        {

            var animation = TransitionState.GetAnimation(Easing.CubicOut);
            var contents = to.MakeWidget(visualState);
            if (UseHeightTransition)
                contents = new HeightTransition(animation, contents, true);
            //if (!animation.Completed) contents = new AnimatedWidget(animation, contents);
            contents = new FadeInElement(animation, contents);

            if (UseHeightTransition)
                contents = Align.Center(contents);

            if (!animation.Completed)
                contents = new AnimatedWidget(animation, contents);

            return new Container(ContainerLayout.Expand,

                from.MakeWidget(visualState),

                new HitTestStop(),

                GestureDetector.TapDetector(visualState, "overlay", () => navigationContainer.Pop(to), Rectangle.Fill(MaskColor.WithOpacity(animation.GetValue()))),

                contents
            );

        }


    }
}
