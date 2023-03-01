using FluidSharp.Layouts;
using FluidSharp.Navigation;
using FluidSharp.Paint;
using FluidSharp.State;
using FluidSharp.Widgets.Caching;
using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using static FluidSharp.Widgets.GestureDetector;

namespace FluidSharp.Widgets
{
    public class StatefulSlideTransition : Widget
    {

        public static TimeSpan DefaultDuration = TimeSpan.FromMilliseconds(250);

        public CarouselState<bool> State;
        public float PanDetectorWidth;

        Func<VisualState, bool, Widget> MakeValueWidget;
        private PanGestureDetector PanDetector;

        public StatefulSlideTransition(VisualState visualState, CarouselState<bool> state, float pandetectorwidth, Func<VisualState, bool, Widget> makeValueWidget) 
        {
            State = state ?? throw new ArgumentNullException(nameof(state));
            PanDetectorWidth = pandetectorwidth;
            MakeValueWidget = makeValueWidget ?? throw new ArgumentNullException(nameof(makeValueWidget));

            var contents = MakeWidget(visualState);
            PanDetector = HorizontalPanDetector(visualState, State, pandetectorwidth, contents);

        }

        WidgetCache Transition = new WidgetCache();
        private Widget MakeWidget(VisualState visualState)
        {
            var frame = State.GetFrame();
            return Transition.Get(frame, () =>
            {
                return SlideTransition.MakeWidget(visualState, frame, 0, PushPageTransition.PushOverlap, null, MakeValueWidget);
            });
        }

        public override SKSize Measure(MeasureCache measureCache, SKSize boundaries) => PanDetector.Child.Measure(measureCache,
            boundaries);

        public override SKRect PaintInternal(LayoutSurface layoutsurface, SKRect rect)
        {

            PanDetector.Child = MakeWidget(layoutsurface.VisualState);
            return layoutsurface.Paint(PanDetector, rect);

        }

    }
}
