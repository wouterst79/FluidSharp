#if false
using FluidSharp.Animations;
using FluidSharp.Layouts;
using FluidSharp.State;
using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace FluidSharp.Widgets.Animations
{

    public class IncrementalAnimationLayoutSurface : LayoutSurface
    {

        public Animation.Coordinated Animation;

        public static TimeSpan DefaultDelta = TimeSpan.FromMilliseconds(100);
        public static TimeSpan DefaultDuration = FluidSharp.Animations.Animation.DefaultDuration;

        private TimeSpan Delta;
        private TimeSpan Duration;

        private int element = 0;

        public IncrementalAnimationLayoutSurface(Animation.Coordinated animation, Device device, MeasureCache measureCache, TimeSpan? delta = default, TimeSpan? duration = default) : base(device, measureCache, null, new VisualState(null, null))
        {
            Animation = animation;
            Delta = delta ?? DefaultDelta;
            Duration = duration ?? DefaultDuration;
        }

        public override SKRect Paint(Widget widget, SKRect rect)
        {
            if (widget is FadeInElement fadeInElement)
            {
                var key = $"element{element++}";
                if (!Animation.TryGetFrame(key, out var frame))
                {
                    frame = Animation.AddFrame(new Frame(key, Delta - Duration, Duration, Easing.CubicOut));
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

        TimeSpan? Delta;
        TimeSpan? Duration;

        private IncrementalAnimationContainer(VisualState visualState, object context, Widget contents, TimeSpan? delta = default, TimeSpan? duration = default)
        {
            if (visualState.NavigationTop.IsContext(context))
            {
                var started = visualState.NavigationTop.Started.AddMilliseconds(500);
                Animation = new Animation.Coordinated(started);
            }
            Contents = contents ?? throw new ArgumentNullException(nameof(contents));
            Delta = delta;
            Duration = duration;
        }

        public static Widget Make(VisualState visualState, object context, Widget contents, TimeSpan? delta = default, TimeSpan? duration = default, Animation? animation = null)
        {
            return contents; // note: set animation if re-activating
            // ncrementalAnimationContainer.Make(visualState, this,
            //printer.PrintScreen(
            //                    background,
            //                    null
            //                   , true
            //                   , PullToRefresh
            //                   ))
            //{ Animation = DiaryScreenState.InAnimation }
        }

        public override SKSize Measure(MeasureCache measureCache, SKSize boundaries) => Contents.Measure(measureCache, boundaries);
        
        public override SKRect PaintInternal(LayoutSurface layoutsurface, SKRect rect)
        {

            if (Animation != null)
            {

                // adding animation frames
                var loadframes = new IncrementalAnimationLayoutSurface(Animation, layoutsurface.Device, layoutsurface.MeasureCache, Delta, Duration);
                loadframes.Paint(Contents, rect);

            }

            // paint contents
            return layoutsurface.Paint(Contents, rect);

        }

    }
}
#endif