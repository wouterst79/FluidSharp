using FluidSharp.Animations;
using FluidSharp.Layouts;
using FluidSharp.State;
using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Text;

namespace FluidSharp.Widgets.Animations
{

    public class IncrementalAnimationLayoutSurface : LayoutSurface
    {

        public Animation.Coordinated Animation;

        private static TimeSpan DefaultDelta = TimeSpan.FromMilliseconds(100);
        private static TimeSpan DefaultDuration = FluidSharp.Animations.Animation.DefaultDuration;

        private int element = 0;

        public IncrementalAnimationLayoutSurface(Animation.Coordinated animation, Device device, MeasureCache measureCache) : base(device, measureCache, null, new VisualState(null, null))
        {
            Animation = animation;
        }

        public override SKRect Paint(Widget widget, SKRect rect)
        {
            if (widget is FadeInElement fadeInElement)
            {
                var key = $"element{element++}";
                if (!Animation.TryGetFrame(key, out var frame))
                {
                    frame = Animation.AddFrame(new Frame(key, DefaultDelta - DefaultDuration, DefaultDuration, Easing.CubicOut));
                }
                fadeInElement.Animation = frame;
            }
            return base.Paint(widget, rect);
        }
    }


    public class IncrementalAnimationContainer : Widget
    {

        public Animation.Coordinated? Animation;
        public Widget Contents { get; set; }

        public IncrementalAnimationContainer(VisualState visualState, object context, Widget contents)
        {
            if (visualState.NavigationTop.IsContext(context))
            {
                var started = visualState.NavigationTop.Started.AddMilliseconds(500);
                Animation = new Animation.Coordinated(started);
            }
            Contents = contents ?? throw new ArgumentNullException(nameof(contents));
        }

        public override SKSize Measure(MeasureCache measureCache, SKSize boundaries) => Contents.Measure(measureCache, boundaries);
        
        public override SKRect PaintInternal(LayoutSurface layoutsurface, SKRect rect)
        {

            if (Animation != null)
            {

                // adding animation frames
                var loadframes = new IncrementalAnimationLayoutSurface(Animation, layoutsurface.Device, layoutsurface.MeasureCache);
                loadframes.Paint(Contents, rect);

            }

            // paint contents
            return layoutsurface.Paint(Contents, rect);

        }

    }
}
