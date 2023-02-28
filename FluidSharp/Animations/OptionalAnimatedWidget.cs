using FluidSharp.Layouts;
using FluidSharp.Widgets;
using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Text;

namespace FluidSharp.Animations
{
    public class OptionalAnimatedWidget : Widget
    {

        public Func<Animation?> Animation;
        public Func<Animation?, Widget?> MakeWidget;

        public OptionalAnimatedWidget(Func<Animation?> animation, Func<Animation?, Widget?> makeWidget)
        {
            Animation = animation ?? throw new ArgumentNullException(nameof(animation));
            MakeWidget = makeWidget ?? throw new ArgumentNullException(nameof(makeWidget));
        }

        private Animation? LastAnimation;
        private Widget? Widget;

        private Widget? GetWidget()
        {
            var a = Animation();
            if (LastAnimation != a)
            {
                Widget = MakeWidget(a);
                LastAnimation = a;
            }
            return Widget;
        }

        public override SKSize Measure(MeasureCache measureCache, SKSize boundaries) => GetWidget()?.Measure(measureCache, boundaries) ?? new SKSize();
        public override SKRect PaintInternal(LayoutSurface layoutsurface, SKRect rect)
        {
            var widget = GetWidget();
            if (widget != null)
                return layoutsurface.Paint(widget, rect);
            else
                return rect.WithHeight(0);
        }
    }
}
