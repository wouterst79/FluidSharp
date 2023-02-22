#if false
using FluidSharp.Navigation;
using FluidSharp.State;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace FluidSharp.Widgets
{
    public class SlideBackNavigation
    {

        public static Widget Make<T>(VisualState visualState, CarouselState<T> stateTransition, TransitionFrame<T> frame, float pandetectorwidth, float spacing, Widget? separator, Func<VisualState, T, Widget> makevaluewidget)
        {
            return GestureDetector.HorizontalPanDetector(visualState, stateTransition, pandetectorwidth,
                        SlideTransition.MakeWidget(visualState, frame, spacing, PushPageTransition.PushOverlap, separator, makevaluewidget)
                   );
        }

    }
}
#endif