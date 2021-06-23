#if DEBUG
//#define DEBUGCONTAINER
#endif
using FluidSharp.Animations;
using FluidSharp.Layouts;
using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Text;

namespace FluidSharp.Widgets.Animations
{
    public class OriginTranslationWidget : Widget
    {

        public class WidgetOrigin : Widget
        {
            public Widget Contents { get; set; }
            public bool PaintOrigin { get; set; }

            public SKRect? OriginRect { get; set; }

            public WidgetOrigin(Widget contents, bool paintOrigin)
            {
                Contents = contents;
                PaintOrigin = paintOrigin;
            }

            public override SKSize Measure(MeasureCache measureCache, SKSize boundaries) => Contents.Measure(measureCache, boundaries);
            public override SKRect PaintInternal(LayoutSurface layoutsurface, SKRect rect)
            {
                if (PaintOrigin)
                {
                    OriginRect = rect = Contents.PaintInternal(layoutsurface, rect);
                }
                else
                {
                    OriginRect = rect = Contents.PaintInternal(new LayoutSurface(layoutsurface.Device, layoutsurface.MeasureCache, null, layoutsurface.VisualState), rect);
#if DEBUGCONTAINER
                    layoutsurface.DebugRect(rect, SKColors.Red);
#endif
                }
                return rect;
            }

        }

        public WidgetOrigin Origin { get; set; }
        public Animation Animation { get; set; }
        public Widget Contents { get; set; }

        public Easing? EasingX { get; set; }
        public Easing? EasingY { get; set; }

        public OriginTranslationWidget(WidgetOrigin origin, Animation animation, Widget contents, Easing? easingX = null, Easing? easingY = null)
        {
            Origin = origin ?? throw new ArgumentNullException(nameof(origin));
            Animation = animation ?? throw new ArgumentNullException(nameof(animation));
            Contents = contents ?? throw new ArgumentNullException(nameof(contents));
            EasingX = easingX;
            EasingY = easingY;
        }

        public override SKSize Measure(MeasureCache measureCache, SKSize boundaries) => Contents.Measure(measureCache, boundaries);
        public override SKRect PaintInternal(LayoutSurface layoutsurface, SKRect rect)
        {

            if (Animation.Completed) return Contents.PaintInternal(layoutsurface, rect);

            if (!Origin.OriginRect.HasValue) throw new Exception("origin must be rendered first");
            var origin = Origin.OriginRect.Value;

            layoutsurface.SetHasActiveAnimations();

            if (true || Animation.Started)
            {

                var value = 1 - Animation.GetValue();
                //var pct = 1 - Animation.GetValue();
                //var pctx = 1 - Easing.SinIn.Ease(value);
                //var pcty = 1 - Easing.SinInOut.Ease(value);
                var pctx = (EasingX is null ? value : EasingX.Ease(value));
                var pcty = (EasingY is null ? value : EasingY.Ease(value));

                // relative to rect (destination)
                var translation = new SKPoint((origin.Left - rect.Left) * pctx, (origin.Top - rect.Top) * pcty);
                var scale = new SKPoint((origin.Width - rect.Width) * pctx, (origin.Width - rect.Width) * pcty);
                var drawrect = new SKRect(rect.Left + translation.X, rect.Top + translation.Y, rect.Right + translation.X + scale.X, rect.Bottom + translation.Y + scale.Y);

                var final = layoutsurface.Paint(Contents, drawrect);
#if DEBUGCONTAINER
                layoutsurface.DebugRect(drawrect, SKColors.Green);
#endif

                return rect.WithHeight(final.Height);

            }
            else
            {

                var childsize = Contents.Measure(layoutsurface.MeasureCache, rect.Size);
                return rect.WithHeight(childsize.Height);

            }

        }
    }

}
