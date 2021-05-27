#if DEBUG
//#define DEBUGHITTEST
#endif
using FluidSharp.Layouts;
using FluidSharp.State;
using FluidSharp.Widgets;
using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FluidSharp.Touch
{
    public class HitTestLayoutSurface : LayoutSurface
    {

        private SKPoint Location;
        private SKPoint Scale = new SKPoint(1, 1);

        public List<HitTestHit> Hits = new List<HitTestHit>();

        private Stack<SKRect> ClipRectStack = new Stack<SKRect>();
        private Stack<SKPath> ClipPathStack;

        public bool HitTestIgnoreClip;

        public override SKRect GetLocalClipRect() => ClipRectStack.Peek();

        public HitTestLayoutSurface(Device device, MeasureCache measureCache, SKPoint location, VisualState visualState, SKRect cliprect) : base(device, measureCache, null, visualState)
        {
            Location = location;
            ClipRectStack.Push(cliprect);
        }

        public override void ClipRect(SKRect cliprect)
        {
            base.ClipRect(cliprect);
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
            var originalClipRects = ClipRectStack;

            if (widget is Scale scale)
            {

                var fx = scale.Factor.X;
                var fy = scale.Factor.Y;

                Location = new SKPoint(Location.X / fx, Location.Y / fy);
                Scale = new SKPoint(Scale.X * fx, Scale.Y * fy);

                if (ClipPathStack != null) throw new Exception("Scaling Clip Paths not supported");

                ClipRectStack = new Stack<SKRect>();
                foreach (var cliprect in originalClipRects.Reverse())
                    ClipRectStack.Push(new SKRect(cliprect.Left / fx, cliprect.Top / fy, cliprect.Right / fx, cliprect.Bottom / fy));

            }

            var hitlocation = Hits.Count;

            // paint the widget tree
            var painted = base.Paint(widget, rect);

#if DEBUGHITTEST
            if (widget is GestureDetector gd)
            {
                System.Diagnostics.Debug.WriteLine($"GD: {gd} {Location} ({rect}) = {painted.Contains(Location)}");
            }
#endif

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

                if (HitTestIgnoreClip) inclip = true;

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
                    if (hitlocation > Hits.Count) hitlocation = Hits.Count;
                    Hits.Insert(hitlocation, new HitTestHit(Device, widget, locationInWidget, rect, Scale));

                }

                HitTestIgnoreClip = false;

            }

            // revert scale factor
            if (widget is Scale)
            {
                Location = originalLocation;
                Scale = originalScale;
                ClipRectStack = originalClipRects;
            }

            // hide earlier hits
            if (widget is HitTestStop)
            {
                Hits.Clear();
            }

            return painted;
        }
    }
}
