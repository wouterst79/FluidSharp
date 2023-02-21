#if false
using FluidSharp.Layouts;
using FluidSharp.Paint;
using FluidSharp.State;
using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace FluidSharp.Widgets
{
    public class StatefulSlideTransition : AnimatedWidget
    {

        public static TimeSpan DefaultDuration = TimeSpan.FromMilliseconds(250);

        public CarouselState<bool> State;
        Func<VisualState, bool, Widget> MakeValueWidget;
        float Overlap;

        private Widget FirstWidget;
        private Widget? ChildA;
        private Widget? ChildB;

        public StatefulSlideTransition(VisualState visualState, CarouselState<bool> state, Func<VisualState, bool, Widget> makeValueWidget, float overlap) : base(new FluidSharp.Animations.Animation(DateTime.Now, TimeSpan.FromSeconds(1)), (Widget)null)
        {
            State = state ?? throw new ArgumentNullException(nameof(state));
            MakeValueWidget = makeValueWidget ?? throw new ArgumentNullException(nameof(makeValueWidget));
            Overlap = overlap;

            var widgets = GetWidgets(visualState);
            FirstWidget = widgets.singlechild;

        }


        private (Widget singlechild, Widget? multichild, float pctOutA, float pctInB) GetWidgets(VisualState visualState)
        {

            var frame = State.GetFrame();
            var ratio = frame.Ratio;

            //System.Diagnostics.Debug.WriteLine($"slide transition frame: {frame.Ratio}");

            var direction = frame.Direction;
            var leftvalue = direction == 1 ? frame.Current : frame.Next;
            var rightvalue = direction == 1 ? frame.Next : frame.Current;

            Widget GetA() => ChildA ?? (ChildA = MakeValueWidget(visualState, leftvalue));
            Widget GetB() => ChildB ?? (ChildB = MakeValueWidget(visualState, rightvalue));

            if (ratio == 0)
                return (direction == 1 ? GetA() : GetB(), null, 0, 0); // MakeValueWidget(visualState, frame.Current);
            else if (ratio == 1)
            {
                if (frame.OnCompleted != null)
                    Task.Run(() => frame.OnCompleted(frame, visualState));
                return (direction == 1 ? GetB() : GetA(), null, 0, 0); // MakeValueWidget(visualState, frame.Next);
            }
            else
            {


                if (direction == -1) ratio = 1 - ratio;

                var extend = 1 - Overlap;

                var pctOutA = ratio * extend;
                var pctInB = ratio;

                //System.Diagnostics.Debug.WriteLine($"{direction}  {pctOutA} {pctInB} ");

                return (GetA(), GetB(), pctOutA, pctInB);

            }

        }

        public override SKSize Measure(MeasureCache measureCache, SKSize boundaries) => FirstWidget.Measure(measureCache,
            boundaries);

        public override SKRect PaintInternal(LayoutSurface layoutsurface, SKRect rect)
        {

            (Widget ChildA, Widget? ChildB, float PctOutA, float PctInB) = GetWidgets(layoutsurface.VisualState);

            if (ChildB is null)
            {
                return layoutsurface.Paint(ChildA, rect);
            }
            else
            {


                layoutsurface.SetHasActiveAnimations();

                var isrtl = layoutsurface.IsRtl;

                var deltaA = (isrtl ? 1 : -1) * rect.Width * PctOutA;
                var rectA = new SKRect(rect.Left + deltaA, rect.Top, rect.Right + deltaA, rect.Bottom);

                var deltaB = (isrtl ? -1 : 1) * rect.Width * (1 - PctInB);
                var rectB = new SKRect(rect.Left + deltaB, rect.Top, rect.Right + deltaB, rect.Bottom);

                var cliprectA = isrtl ?
                                new SKRect(rectB.Right, rect.Top, rectA.Right, rect.Bottom) :
                                new SKRect(rectA.Left, rect.Top, rectB.Left, rect.Bottom);

                // paint child A
                layoutsurface.ClipRect(cliprectA);
                PaintBackground(ChildA, cliprectA);
                layoutsurface.Paint(ChildA, rectA);
                layoutsurface.ResetRectClip();

                // paint child B
                PaintBackground(ChildB, rectB);
                layoutsurface.Paint(ChildB, rectB);

                return rect;

            }

            void PaintBackground(Widget widget, SKRect rect)
            {
                if (layoutsurface.Canvas == null) return;
                if (widget is IBackgroundColorSource backgroundColorSource)
                {
                    var color = backgroundColorSource.GetBackgroundColor(layoutsurface.VisualState);
                    if (color != default)
                    {
                        layoutsurface.Canvas.DrawRect(rect, PaintCache.GetBackgroundPaint(color));
                    }
                }
            }

        }

    }
}
#endif