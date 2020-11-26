using FluidSharp.Animations;
using FluidSharp.Layouts;
using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace FluidSharp.Widgets
{
    public class AnimatedWidget : Widget
    {

        public Animation Animation;

        public Widget? Child;

        public AnimatedWidget(Animation animation, Widget? child)
        {
            Animation = animation;
            Child = child;
        }

        public AnimatedWidget(Animation animation, Func<float, Widget?> makechild)
        {
            Animation = animation;
            var value = animation.GetValue();
            Child = makechild(value);
        }

        public override SKSize Measure(MeasureCache measureCache, SKSize boundaries) => Child?.Measure(measureCache, boundaries) ?? new SKSize();
        public override SKRect PaintInternal(LayoutSurface layoutsurface, SKRect rect)
        {
            // don't do it here, because this class may be inherited
            //if (!Animation.Completed) layoutsurface.SetHasActiveAnimations();
            return Child == null ? rect : layoutsurface.Paint(Child, rect);
        }
    }
}
