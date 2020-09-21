using FluidSharp.State;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace FluidSharp.Widgets
{
    public class Carousel
    {

        //public static Widget Make<T>(VisualState visualState, CarouselState<T> stateTransition, Func<VisualState, T, Widget> makevaluewidget)
        //{
        //    return GestureDetector.HorizontalPanDetector(visualState, stateTransition,
        //                SlideTransition.MakeWidget(visualState, stateTransition.GetFrame(), makevaluewidget)
        //           );
        //}

        public static Widget Make<T>(VisualState visualState, CarouselState<T> stateTransition, TransitionFrame<T> frame, float spacing, Widget separator, Func<VisualState, T, Widget> makevaluewidget)
        {
            return GestureDetector.HorizontalPanDetector(visualState, stateTransition,
                        SlideTransition.MakeWidget(visualState, frame, spacing, separator, makevaluewidget)
                   );
        }

    }
}
