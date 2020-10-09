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

        public OverlayPageTransition(SKColor maskColor, Func<bool, Task> onTransitionCompleted)
        {
            MaskColor = maskColor;
            OnTransitionCompleted2 = onTransitionCompleted;
            TransitionState = new NavigationTransitionState(false, OnTransitionCompleted);
        }

        public Task OnTransitionCompleted(bool open) => !open ? OnTransitionCompleted2(open) : Task.CompletedTask;

        public Widget MakeWidget(VisualState visualState, IWidgetSource from, IWidgetSource to, Func<Task> dismiss)
        {

            var animation = TransitionState.GetAnimation(Easing.CubicOut);
            var popup = new HeightTransition(animation, to.MakeWidget(visualState));

            return new Layout()
            {

                Rows =
                {
                    new LayoutSize.Remaining(),
                    new LayoutSize.Fit()
                },
                Cells =
                {
                    new LayoutCell(0,0,1,2, from.MakeWidget(visualState)),
                    new LayoutCell(0,0, new HitTestStop()),
                    new LayoutCell(0,0, GestureDetector.TapDetector(visualState, "overlay", dismiss, Rectangle.Fill(MaskColor.WithOpacity(animation.GetValue())))),
                    new LayoutCell(0,1, Rectangle.Fill(MaskColor.WithOpacity(animation.GetValue()))),
                    new LayoutCell(0,1, popup),
                }
            };

        }


    }
}
