#if false
using FluidSharp.Animations;
using FluidSharp.Layouts;
using FluidSharp.State;
using FluidSharp.Touch;
using FluidSharp.Widgets;
using FluidSharp.Widgets.Material;
using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace FluidSharp.Navigation
{
    public class OverlayPageTransition : IPageTransition
    {

        public SKColor MaskColor;

        public NavigationTransitionState TransitionState;
        private Func<bool, Task> OnTransitionCompleted2;

        public Task Start() => TransitionState.SetTarget(true, null);
        public Task Reverse() => TransitionState.SetTarget(false, null);

        public OverlayPageTransition(bool startingstate, SKColor maskColor, Func<bool, Task> onTransitionCompleted)
        {
            MaskColor = maskColor;
            OnTransitionCompleted2 = onTransitionCompleted;
            TransitionState = new NavigationTransitionState(startingstate, onTransitionCompleted);
            //TransitionState.TransitionDuration = TimeSpan.FromSeconds(2);
        }

        public Task OnTransitionCompleted(bool open) => !open ? OnTransitionCompleted2(open) : Task.CompletedTask;

        public Widget MakeWidget(VisualState visualState, IWidgetSource from, IWidgetSource to, Func<Task> dismiss)
        {

            var animation = TransitionState.GetAnimation(Easing.CubicOut);
            var popup = to.MakeWidget(visualState);
            popup = new HeightTransition(animation, popup, false);
            popup = new Opacity(animation.GetValue(), popup);

            return new Container(ContainerLayout.Expand, 

                from.MakeWidget(visualState),
                new HitTestStop(),
                Rectangle.Fill(MaskColor.WithOpacity(animation.GetValue())),

                new SplitContainer(
                    popup,
                    GestureDetector.TapDetector(visualState, "overlay", dismiss, new Container(ContainerLayout.Expand))
                )

            );

        }


    }
}
#endif