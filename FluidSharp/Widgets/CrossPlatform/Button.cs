using FluidSharp;
using FluidSharp.Layouts;
using FluidSharp.State;
using FluidSharp.Widgets;
using FluidSharp.Widgets.Stateful;
using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading.Tasks;

namespace FluidSharp.Widgets.CrossPlatform
{
    public class Button : Widget
    {

        private Widget NotTouchedState { get; set; }
        private Widget TouchedState { get; set; }

        private object Context;

        public Button(VisualState visualState, SKColor selectedBackgroundColor, object context, Func<Task> onTapped, Widget child)
            : this(visualState, selectedBackgroundColor, context, onTapped, null, child)
        {
        }

        public Button(VisualState visualState, SKColor selectedBackgroundColor, object context, Func<Task> onTapped, Func<Task>? onLongTapped, Widget child)
        {

            Context = context;

            NotTouchedState = GestureDetector.TapDetector(visualState, context, onTapped, onLongTapped, child);

#if DEBUG
            if (child is Container c1 && !c1.FillHorizontal)
                Debug.WriteLine("CHILD NOT FILL");
#endif

            var layout = child is Container c && c.FillVertical ? ContainerLayout.Fill : ContainerLayout.FillHorizontal;
            // ? c.GetContainerLayout() : ContainerLayout.Fill;
            if (child is Center || child is Align) layout = ContainerLayout.Fill;
            if (child is TwoStateWidget) layout = ContainerLayout.Fill;


            var fill = Rectangle.Fill(selectedBackgroundColor);
            TouchedState = GestureDetector.TapDetector(visualState, context, onTapped, onLongTapped,
                                        new Container(layout, child, fill));

        }

        public override SKSize Measure(MeasureCache measureCache, SKSize boundaries) => NotTouchedState.Measure(measureCache, boundaries);

        public override SKRect PaintInternal(LayoutSurface layoutsurface, SKRect rect)
        {
            if (layoutsurface.VisualState.TouchTarget.IsContext<TapContext>(Context, false))
                return layoutsurface.Paint(TouchedState, rect);
            else
                return layoutsurface.Paint(NotTouchedState, rect);
        }
    }
}
