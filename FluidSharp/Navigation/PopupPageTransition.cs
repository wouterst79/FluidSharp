using FluidSharp.Animations;
using FluidSharp.Layouts;
using FluidSharp.State;
using FluidSharp.Touch;
using FluidSharp.Widgets;
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

        public Task Start() => TransitionState.SetTarget(true, null);
        public Task Reverse() => TransitionState.SetTarget(false, null);

        public PopupPageTransition(SKColor maskColor, Func<bool, Task> onTransitionCompleted)
        {
            MaskColor = maskColor;
            OnTransitionCompleted2 = onTransitionCompleted;
            TransitionState = new NavigationTransitionState(false, OnTransitionCompleted);
        }

        public Task OnTransitionCompleted(bool open) => !open ? OnTransitionCompleted2(open) : Task.CompletedTask;

        public Widget MakeWidget(VisualState visualState, IWidgetSource from, IWidgetSource to, Func<Task> dismiss)
        {

            var animation = TransitionState.GetAnimation(Easing.CubicOut);
            var contents = new HeightTransition(animation, to.MakeWidget(visualState));

            return new Container(ContainerLayout.Expand,

                from.MakeWidget(visualState),

                new HitTestStop(),

                GestureDetector.TapDetector(visualState, "overlay", dismiss, Rectangle.Fill(MaskColor.WithOpacity(animation.GetValue()))),

                Align.Center(
                    contents
                    )
            );

        }


    }
}
