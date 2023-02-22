using FluidSharp.Layouts;
using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Text;

namespace FluidSharp.Widgets.Caching
{
    public class CachedWidget<TValue> : Widget
    {

        private Func<TValue> GetValue;
        private Func<TValue, Widget> MakeWidget;

        private TValue LastValue;
        private Widget? Widget;

        private Widget GetWidget()
        {
            var value = GetValue();
#if DEBUG
            if (value == null) throw new ArgumentOutOfRangeException("value");
#endif
            if (Widget is null || !value.Equals(LastValue))
            {
                Widget = MakeWidget(value);
            }
            return Widget;
        }

        public CachedWidget(Func<TValue> getValue, Func<TValue, Widget> makeWidget)
        {
            GetValue = getValue ?? throw new ArgumentNullException(nameof(getValue));
            MakeWidget = makeWidget ?? throw new ArgumentNullException(nameof(makeWidget));
            LastValue = getValue();
            Widget = MakeWidget(LastValue);
        }

        public override SKSize Measure(MeasureCache measureCache, SKSize boundaries) => GetWidget().Measure(measureCache, boundaries);
        public override SKRect PaintInternal(LayoutSurface layoutsurface, SKRect rect) => layoutsurface.Paint(GetWidget(), rect);

    }
}
