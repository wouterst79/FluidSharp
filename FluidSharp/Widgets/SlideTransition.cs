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
    public class SlideTransition : AnimatedWidget
    {

        public static TimeSpan DefaultDuration = TimeSpan.FromMilliseconds(250);

        public Widget ChildA;
        public Widget ChildB;

        public float PctOutA;
        public float PctInB;


        public SlideTransition(Widget childA, Widget childB, float pctOutA, float pctInB) : base(new FluidSharp.Animations.Animation(DateTime.Now, TimeSpan.FromSeconds(1)), (Widget)null)
        {
            ChildA = childA ?? throw new ArgumentNullException(nameof(childA));
            ChildB = childB ?? throw new ArgumentNullException(nameof(childB));
            PctOutA = pctOutA;
            PctInB = pctInB;
        }

        public static Widget MakeWidget<T>(VisualState visualState, TransitionFrame<T> frame, float overlap, Func<VisualState, T, Widget> makevaluewidget)
            => MakeWidget(visualState, frame, 0, overlap, null, makevaluewidget);

        public static Widget MakeWidget<T>(VisualState visualState, TransitionFrame<T> frame, float spacing, float overlap, Widget separator, Func<VisualState, T, Widget> makevaluewidget)
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

                if (direction == -1) ratio = 1 - ratio;

                var extend = 1 - overlap;

                var pctOutA = ratio * extend;
                var pctInB = ratio;

                //System.Diagnostics.Debug.WriteLine($"{direction}  {pctOutA} {pctInB} ");

                var leftvalue = direction == 1 ? frame.Current : frame.Next;
                var rightvalue = direction == 1 ? frame.Next : frame.Current;

                var leftchild = makevaluewidget(visualState, leftvalue);
                var rightchild = makevaluewidget(visualState, rightvalue);

                return new SlideTransition(leftchild, rightchild, pctOutA, pctInB);

            }

        }

        public override SKSize Measure(MeasureCache measureCache, SKSize boundaries) => ChildA.Measure(measureCache,
            boundaries);
        public override SKRect PaintInternal(LayoutSurface layoutsurface, SKRect rect)
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

            // paint child B
            layoutsurface.ClipRect(cliprectA);
            PaintBackground(ChildA, cliprectA);
            layoutsurface.Paint(ChildA, rectA);
            layoutsurface.ResetRectClip();

            // paint child B
            PaintBackground(ChildB, rectB);
            layoutsurface.Paint(ChildB, rectB);

            return rect;

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
