using FluidSharp.State;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace FluidSharp.Widgets
{
    public class Carousel
    {

        public static Widget Make<T>(VisualState visualState, CarouselState<T> stateTransition, TransitionFrame<T> frame, float spacing, Widget separator, Func<T, Widget> makevaluewidget, Func<Task> onCompleted)
        {
            return GestureDetector.HorizontalPanDetector(visualState, stateTransition,
                        SlideTransition.MakeWidget(frame, spacing, separator, makevaluewidget, onCompleted)
                   );
        }

    }
}
