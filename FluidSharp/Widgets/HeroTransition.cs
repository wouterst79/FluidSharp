using FluidSharp.Layouts;
using FluidSharp.State;
using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace FluidSharp.Widgets
{

    public class HeroTransition : Widget
    {


        private class HeroLocator : LayoutSurface
        {

            public Dictionary<string, SKRect> HeroLocations = new Dictionary<string, SKRect>();
            public HeroLocator(Device device, MeasureCache measureCache, VisualState visualState) : base(device, measureCache, null, visualState)
            {
            }

            public override SKRect Paint(Widget widget, SKRect rect)
            {
                var result = base.Paint(widget, rect);
                if (widget is Hero hero)
                    HeroLocations.Add(hero.Tag, result);
                return result;
            }
        }

        private class HeroPositioner : LayoutSurface
        {

            public Dictionary<string, SKRect> HeroLocations = new Dictionary<string, SKRect>();
            public HeroPositioner(Device device, MeasureCache measureCache, SKCanvas canvas, VisualState visualState) : base(device, measureCache, canvas, visualState)
            {
            }

            public override SKRect Paint(Widget widget, SKRect rect)
            {
                if (widget is Hero hero)
                    if (HeroLocations.TryGetValue(hero.Tag, out var target))
                    {
                        rect = target;
                        return base.Paint(new Container(ContainerLayout.Fill, new Margins(16,0), new RoundedRectangle(8, SKColors.White, default)), rect);
                    }
                return base.Paint(widget, rect);
            }
        }


        public static TimeSpan DefaultDuration = TimeSpan.FromMilliseconds(2500);

        public Widget ChildA;
        public Widget ChildB;

        public float PctA;

        public HeroTransition(Widget childA, Widget childB, float pctA)
        {
            ChildA = childA ?? throw new ArgumentNullException(nameof(childA));
            ChildB = childB ?? throw new ArgumentNullException(nameof(childB));
            this.PctA = pctA;
        }

        public static Widget MakeWidget<T>(VisualState visualState, TransitionFrame<T> frame, Func<VisualState, T, Widget> makevaluewidget)
            => MakeWidget(visualState, frame, 0, null, makevaluewidget);

        public static Widget MakeWidget<T>(VisualState avalue, TransitionFrame<T> frame, float spacing, Widget separator, Func<VisualState, T, Widget> makevaluewidget)
        {

            var ratio = frame.Ratio;

            //System.Diagnostics.Debug.WriteLine($"slide transition frame: {frame.Ratio}");

            if (ratio == 0)
                return makevaluewidget(avalue, frame.Current);
            else if (ratio == 1)
            {
                if (frame.OnCompleted != null)
                    Task.Run(() => frame.OnCompleted(frame, avalue));
                return makevaluewidget(avalue, frame.Next);
            }
            else
            {

                var direction = frame.Direction;

                var min = direction == 1 ? 1 : 0;
                var delta = direction == 1 ? -1 : 1;

                var amount = min + ratio * delta;

                var leftvalue = direction == 1 ? frame.Current : frame.Next;
                var bvalue = direction == 1 ? frame.Next : frame.Current;

                return new HeroTransition(makevaluewidget(avalue, leftvalue), makevaluewidget(avalue, bvalue), amount);

            }

        }

        public override SKSize Measure(MeasureCache measureCache, SKSize boundaries) => ChildA.Measure(measureCache,
            boundaries);

        public override SKRect PaintInternal(LayoutSurface layoutsurface, SKRect rect)
        {

            if (layoutsurface.Canvas == null)
            {
                return layoutsurface.Paint(ChildA, rect);
            }

            layoutsurface.SetHasActiveAnimations();

            var locatorA = new HeroLocator(layoutsurface.Device, layoutsurface.MeasureCache, layoutsurface.VisualState);
            locatorA.Paint(ChildA, rect);

            var locatorB = new HeroLocator(layoutsurface.Device, layoutsurface.MeasureCache, layoutsurface.VisualState);
            locatorB.Paint(ChildB, rect);

            var pctA = PctA;
            var pctB = 1 - pctA;

            var locations = new Dictionary<string, SKRect>();
            foreach (var location in locatorA.HeroLocations)
            {
                var source = location.Value;
                var target = locatorB.HeroLocations[location.Key];

                var herorect = new SKRect(source.Left * pctA + target.Left * pctB,
                    source.Top * pctA + target.Top * pctB,
                    source.Right * pctA + target.Right * pctB,
                    source.Bottom * pctA + target.Bottom * pctB
                    );

                locations.Add(location.Key, herorect);
            }

            var positioner = new HeroPositioner(layoutsurface.Device, layoutsurface.MeasureCache, layoutsurface.Canvas, layoutsurface.VisualState) { HeroLocations = locations };
            return positioner.Paint(ChildA, rect);
        }

    }
}
