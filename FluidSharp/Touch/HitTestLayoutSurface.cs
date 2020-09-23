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

        private Stack<SKRect> ClipRectStack;
        private Stack<SKPath> ClipPathStack;

        public HitTestLayoutSurface(Device device, MeasureCache measureCache, SKPoint location, VisualState visualState) : base(device, measureCache, null, visualState)
        {
            Location = location;
        }

        public override void ClipRect(SKRect cliprect)
        {
            base.ClipRect(cliprect);
            if (ClipRectStack == null) ClipRectStack = new Stack<SKRect>();
            ClipRectStack.Push(cliprect);
        }

        public override void ResetRectClip()
        {
            base.ResetRectClip();
            ClipRectStack.Pop();
        }

        public override void ClipPath(SKPath clipPath)
        {
            base.ClipPath(clipPath);
            if (ClipPathStack == null) ClipPathStack = new Stack<SKPath>();
            ClipPathStack.Push(clipPath);
        }

        public override void ResetPathClip()
        {
            base.ResetPathClip();
            ClipPathStack.Pop().Dispose();
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

            var hitlocation = Hits.Count;

            // paint the widget tree
            var painted = base.Paint(widget, rect);

            // hit testing
            if (painted.Contains(Location))
            {

                var inclip = true;
                if (ClipRectStack != null)
                {
                    foreach (var cliprect in ClipRectStack)
                        if (!cliprect.Contains(Location))
                        {
                            inclip = false;
                            break;
                        }
                }
                if (ClipPathStack != null && inclip)
                {
                    foreach (var clippath in ClipPathStack)
                        if (!clippath.Contains(Location.X, Location.Y))
                        {
                            inclip = false;
                            break;
                        }
                }

                if (inclip)
                {

#if DEBUGHITTEST
                    if (widget != null)
                        if (widget is GestureDetector gd)
                            System.Diagnostics.Debug.WriteLine($"hit test hit ({hitlocation} / {Hits.Count}): {widget.GetType().Name}: {rect} ({Location}) {{{gd.Context}}}");
//                        else
  //                          System.Diagnostics.Debug.WriteLine($"hit test hit: {widget.GetType().Name}: {rect} ({Location})");
#endif

                    var locationInWidget = new SKPoint(Location.X - rect.Left, Location.Y - rect.Top);
                    Hits.Insert(hitlocation, new HitTestHit(Device, widget, locationInWidget, rect, Scale));

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
