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

        public IAnimation Animation;

        public Widget? Contents;

        public Action<float>? UpdateContents;

        public AnimatedWidget(IAnimation animation, Widget? contents)
        {
            Animation = animation;
            Contents = contents;
        }

        public AnimatedWidget(IAnimation animation, Widget? contents, Action<float> updateContents)
        {
            Animation = animation;
            Contents = contents;
            UpdateContents = updateContents;
        }


        public override SKSize Measure(MeasureCache measureCache, SKSize boundaries) => Contents?.Measure(measureCache, boundaries) ?? new SKSize();
        public override SKRect PaintInternal(LayoutSurface layoutsurface, SKRect rect)
        {
            if (UpdateContents != null)
            {
                UpdateContents(Animation.GetValue());
            }
            // don't do it here, because this class may be inherited
            //if (!Animation.Completed) layoutsurface.SetHasActiveAnimations();
            return Contents == null ? rect : layoutsurface.Paint(Contents, rect);
        }
    }
}
