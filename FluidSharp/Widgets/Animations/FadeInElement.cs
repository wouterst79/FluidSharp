using FluidSharp.Animations;
using FluidSharp.Layouts;
using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Text;

namespace FluidSharp.Widgets.Animations
{
    public class FadeInElement : Widget
    {

        //public Widget? Contents;

        public Animation? Animation { get; set; }
        public Opacity? Opacity { get; set; }

        private FadeInElement(Widget contents)
        {
            Opacity = new Opacity(1, contents);
        }


        public override SKSize Measure(MeasureCache measureCache, SKSize boundaries) => Opacity.Measure(measureCache, boundaries);
        public override SKRect PaintInternal(LayoutSurface layoutsurface, SKRect rect)
        {
            if (Animation != null)
            {
                Opacity.Factor = Animation.GetValue();
                if (!Animation.Completed)
                    layoutsurface.SetHasActiveAnimations();
            }
            return layoutsurface.Paint(Opacity, rect);
        }

        public static Widget Make(Widget contents)
        {
            return contents;
            //return new FadeInElement(contents);
        }

    }


    //public class FadeInElement2 : Widget
    //{

    //    public Widget Contents;

    //    public Animation? Animation { get; set; }
    //    public Action<Animation, Widget> SetAnimation { get; set; }

    //    public FadeInElement2(Widget contents, Action<Animation, Widget> setAnimation)
    //    {
    //        Contents = contents;
    //        SetAnimation = setAnimation;
    //    }


    //    public override SKSize Measure(MeasureCache measureCache, SKSize boundaries) => Contents.Measure(measureCache, boundaries);
    //    public override SKRect PaintInternal(LayoutSurface layoutsurface, SKRect rect)
    //    {
    //        if (Animation != null)
    //        {
    //            Opacity.Factor = Animation.GetValue();
    //            if (!Animation.Completed)
    //                layoutsurface.SetHasActiveAnimations();
    //        }
    //        return layoutsurface.Paint(Opacity, rect);
    //    }
    //}
}
