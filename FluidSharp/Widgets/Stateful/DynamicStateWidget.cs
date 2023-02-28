using FluidSharp.Layouts;
using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Text;

namespace FluidSharp.Widgets.Stateful
{

    public interface IDynamicWidget
    {
        public void Reset();
    }


    public class DynamicStateWidget<T> : Widget, IDynamicWidget
    {

        public Func<T> GetState;
        public Func<T, Widget?> MakeWidget;

        public DynamicStateWidget(Func<T> getState, Func<T, Widget?> makeWidget)
        {
            GetState = getState ?? throw new ArgumentNullException(nameof(getState));
            MakeWidget = makeWidget ?? throw new ArgumentNullException(nameof(makeWidget));
        }

        private T LastState;
        private Widget? LastWidget;

        public void Reset() => LastState = default!;

        private Widget? GetWidget()
        {
            var state = GetState();
            if ((state is null && !(LastState is null)) ||
                (!(state is null) && !state.Equals(LastState)))
            {
                LastWidget = MakeWidget(state);
                LastState = state;
            }
            return LastWidget;
        }

        public override SKSize Measure(MeasureCache measureCache, SKSize boundaries) => GetWidget()?.Measure(measureCache, boundaries) ?? new SKSize();
        public override SKRect PaintInternal(LayoutSurface layoutsurface, SKRect rect)
        {
            var widget = GetWidget();
            if (widget is null) return rect.WithHeight(0);
            else return layoutsurface.Paint(widget, rect);
        }

    }
}
