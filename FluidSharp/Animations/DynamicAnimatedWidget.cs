using FluidSharp.Layouts;
using FluidSharp.Widgets;
using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Text;

namespace FluidSharp.Animations
{
    public class DynamicAnimatedWidget<T> : Widget where T : Widget
    {

        public Func<Animation?> Animation1;
        public Func<Animation, T, Widget> MakeWidget1;

        public Func<Animation?>? Animation2;
        public Func<Animation, T, Widget>? MakeWidget2;

        public Func<Animation?>? Animation3;
        public Func<Animation, T, Widget>? MakeWidget3;

        public T DefaultWidget;

        public DynamicAnimatedWidget(Func<Animation?> animation1, Func<Animation, T, Widget> makeWidget1, T defaultWidget)
        {
            Animation1 = animation1 ?? throw new ArgumentNullException(nameof(animation1));
            MakeWidget1 = makeWidget1 ?? throw new ArgumentNullException(nameof(makeWidget1));
            DefaultWidget = defaultWidget;
        }

        public DynamicAnimatedWidget(Func<Animation?> animation1, Func<Animation, T, Widget> makeWidget1, Func<Animation?> animation2, Func<Animation, T, Widget> makeWidget2, T defaultWidget)
        {
            Animation1 = animation1 ?? throw new ArgumentNullException(nameof(animation1));
            MakeWidget1 = makeWidget1 ?? throw new ArgumentNullException(nameof(makeWidget1));
            Animation2 = animation2;
            MakeWidget2 = makeWidget2;
            DefaultWidget = defaultWidget;
        }

        public DynamicAnimatedWidget(Func<Animation?> animation1, Func<Animation, T, Widget> makeWidget1, Func<Animation?>? animation2, Func<Animation, T, Widget>? makeWidget2, Func<Animation?>? animation3, Func<Animation, T, Widget>? makeWidget3, T defaultWidget)
        {
            Animation1 = animation1 ?? throw new ArgumentNullException(nameof(animation1));
            MakeWidget1 = makeWidget1 ?? throw new ArgumentNullException(nameof(makeWidget1));
            Animation2 = animation2;
            MakeWidget2 = makeWidget2;
            Animation3 = animation3;
            MakeWidget3 = makeWidget3;
            DefaultWidget = defaultWidget;
        }

        private Animation? LastAnimation;
        private Widget? LastWidget;

        private Widget GetWidget()
        {
            var a1 = Animation1();
            if (a1 != null)
            {
                if (LastAnimation != a1) LastWidget = null;
                if (LastWidget is null)
                {
                    LastWidget = MakeWidget1(a1, DefaultWidget);
                    LastAnimation = a1;
                }
                return LastWidget;
            }
            if (Animation2 != null && MakeWidget2 != null)
            {
                var a2 = Animation2();
                if (a2 != null)
                {
                    if (LastAnimation != a2) LastWidget = null;
                    if (LastWidget is null)
                    {
                        LastWidget = MakeWidget2(a2, DefaultWidget);
                        LastAnimation = a2;
                    }
                    return LastWidget;
                }
            }
            if (Animation3 != null && MakeWidget3 != null)
            {
                var a3 = Animation3();
                if (a3 != null)
                {
                    if (LastAnimation != a3) LastWidget = null;
                    if (LastWidget is null)
                    {
                        LastWidget = MakeWidget3(a3, DefaultWidget);
                        LastAnimation = a3;
                    }
                    return LastWidget;
                }
            }
            return DefaultWidget;
        }

        public override SKSize Measure(MeasureCache measureCache, SKSize boundaries) => GetWidget().Measure(measureCache, boundaries);
        public override SKRect PaintInternal(LayoutSurface layoutsurface, SKRect rect) => layoutsurface.Paint(GetWidget(), rect);

    }
}
