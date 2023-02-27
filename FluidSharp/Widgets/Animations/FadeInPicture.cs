using FluidSharp.Animations;
using FluidSharp.Layouts;
using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Text;

namespace FluidSharp.Widgets.Animations
{
    public class FadeInPicture : Widget
    {

        public IAnimation Animation { get; set; }
        public Picture Picture { get; set; }

        public FadeInPicture(IAnimation animation, Picture picture)
        {
            Animation = animation;
            var startingOpacity = animation.GetValue();
            Picture = picture.WithOpacity(startingOpacity);
        }



        public override SKSize Measure(MeasureCache measureCache, SKSize boundaries) => Picture.Measure(measureCache, boundaries);
        public override SKRect PaintInternal(LayoutSurface layoutsurface, SKRect rect)
        {
            Picture.Opacity = Animation.GetValue();
            if (!Animation.Completed)
                layoutsurface.SetHasActiveAnimations();
            return layoutsurface.Paint(Picture, rect);
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
