using FluidSharp.Animations;
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
    public class PopupPageTransition : IPageTransition
    {

        public SKColor MaskColor;
        private readonly float CornerRadius;
        private readonly SKColor BackgroundColor;
        private readonly Margins InnerMargins;
        public NavigationTransitionState TransitionState;
        private Func<bool, Task> OnTransitionCompleted2;

        public Task Start() => TransitionState.SetTarget(true, null);
        public Task Reverse() => TransitionState.SetTarget(false, null);

        public PopupPageTransition(SKColor maskColor, float cornerRadius, SKColor backgroundColor, Margins innerMargins, Func<bool, Task> onTransitionCompleted)
        {
            MaskColor = maskColor;
            CornerRadius = cornerRadius;
            BackgroundColor = backgroundColor;
            InnerMargins = innerMargins;
            OnTransitionCompleted2 = onTransitionCompleted;
            TransitionState = new NavigationTransitionState(false, OnTransitionCompleted);
        }

        public Task OnTransitionCompleted(bool open) => !open ? OnTransitionCompleted2(open) : Task.CompletedTask;

        public Widget MakeWidget(VisualState visualState, IWidgetSource from, IWidgetSource to)
        {

            var animation = TransitionState.GetAnimation(Easing.CubicOut);
            var contents = new HeightTransition(animation, to.MakeWidget(visualState));

            return new Container(ContainerLayout.Expand,

                from.MakeWidget(visualState),

                Rectangle.Fill(MaskColor.WithOpacity(animation.GetValue())),

                Align.Center(
                    new RoundedRectangle(CornerRadius, BackgroundColor, default,
                        contents
                    )
                )
            );

        }


    }
}
