using FluidSharp.Layouts;
using FluidSharp.State;
using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace FluidSharp.Widgets
{
    public class SlideTransition : Widget
    {

        public static TimeSpan DefaultDuration = TimeSpan.FromMilliseconds(250);

        public Widget Child;

        public static Widget MakeWidget<T>(VisualState visualState, TransitionFrame<T> frame, Func<VisualState, T, Widget> makevaluewidget)
            => MakeWidget(visualState, frame, 0, null, makevaluewidget);

        public static Widget MakeWidget<T>(VisualState visualState, TransitionFrame<T> frame, float spacing, Widget separator, Func<VisualState, T, Widget> makevaluewidget)
        {

            var ratio = frame.Ratio;

            //System.Diagnostics.Debug.WriteLine($"slide transition frame: {frame.Ratio}");

            if (ratio == 0)
                return makevaluewidget(visualState, frame.Current);
            else if (ratio == 1)
            {
                if (frame.OnCompleted != null)
                    Task.Run(() => frame.OnCompleted(frame, visualState));
                return makevaluewidget(visualState, frame.Next);
            }
            else
            {

                var direction = frame.Direction;

                var min = direction == 1 ? 1 : 0;
                var delta = direction == 1 ? -1 : 1;

                var amount = min + ratio * delta;

                var leftvalue = direction == 1 ? frame.Current : frame.Next;
                var rightvalue = direction == 1 ? frame.Next : frame.Current;

                return new SlideTransition()
                {
                    Child =
                    new Layout()
                    {

                        Rows = { new LayoutSize.Fit() },
                        //Debug = true,
                        Columns =
                        {
                            new LayoutSize.Available(amount - 1),
                            new LayoutSize.Absolute(-spacing / 2),
                            new LayoutSize.Available(1),
                            new LayoutSize.Absolute(spacing),
                            new LayoutSize.Available(1)
                        },

                        Cells =
                        {

                            separator == null ? null :
                            new LayoutCell(3,0, separator),

                            new LayoutCell(2,0, makevaluewidget(visualState, leftvalue)),
                            new LayoutCell(4,0, makevaluewidget(visualState, rightvalue))
                        }
                    }
                };
            }

        }

        public override SKSize Measure(MeasureCache measureCache, SKSize boundaries) => Child.Measure(measureCache,
            boundaries);
        public override SKRect PaintInternal(LayoutSurface layoutsurface, SKRect rect)
        {
            layoutsurface.SetHasActiveAnimations();
            return layoutsurface.Paint(Child, rect);
        }

    }
}
