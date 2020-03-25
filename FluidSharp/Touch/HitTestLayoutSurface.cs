#if DEBUG
//#define DEBUGHITTEST
#endif
using FluidSharp.Layouts;
using FluidSharp.State;
using FluidSharp.Widgets;
using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Text;

namespace FluidSharp.Touch
{
    public class HitTestLayoutSurface : LayoutSurface
    {

        private SKPoint Location;
        private SKPoint Scale = new SKPoint(1, 1);

        public List<HitTestHit> Hits = new List<HitTestHit>();

        private Stack<SKRect> ClipRectStack = new Stack<SKRect>();

        public HitTestLayoutSurface(Device device, MeasureCache measureCache, SKPoint location, VisualState visualState) : base(device, measureCache, null, visualState)
        {
            Location = location;
        }

        public override void ClipRect(SKRect cliprect)
        {
            base.ClipRect(cliprect);
            ClipRectStack.Push(cliprect);
        }

        public override void ResetClip()
        {
            base.ResetClip();
            ClipRectStack.Pop();
        }

        public override SKRect Paint(Widget widget, SKRect rect)
        {

            // scale
            var originalLocation = Location;
            var originalScale = Scale;

            if (widget is Scale scale)
            {
                Location = new SKPoint(Location.X / scale.Factor.X, Location.Y / scale.Factor.Y);
                Scale = new SKPoint(Scale.X * scale.Factor.X, Scale.Y * scale.Factor.Y);
            }

            // paint the widget tree
            var painted = base.Paint(widget, rect);

            // hit testing
            if (painted.Contains(Location))
            {

                var inclip = true;
                if (ClipRectStack.Count > 0)
                {
                    foreach (var cliprect in ClipRectStack)
                        if (!cliprect.Contains(Location))
                        {
                            inclip = false;
                            break;
                        }
                }

                if (inclip)
                {

#if DEBUGHITTEST
                    if (widget != null)
//                        if (widget is GestureDetector)
                            System.Diagnostics.Debug.WriteLine($"hit test hit: {widget.GetType().Name}: {rect} ({Location})");
#endif

                    var locationInWidget = new SKPoint(Location.X - rect.Left, Location.Y - rect.Top);
                    Hits.Add(new HitTestHit(Device, widget, locationInWidget, rect, Scale));

                }
            }

            // revert scale factor
            if (widget is Scale)
            {
                Location = originalLocation;
                Scale = originalScale;
            }

            return painted;
        }
    }
}
