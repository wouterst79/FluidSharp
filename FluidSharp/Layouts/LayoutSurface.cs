#if DEBUG
//#define DEBUGCONTAINER
#endif
using FluidSharp.Engine;
using FluidSharp.Layouts;
using FluidSharp.State;
using FluidSharp.Widgets;
using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Text;

namespace FluidSharp
{
    public class LayoutSurface
    {

        public Device Device;
        public MeasureCache MeasureCache;
        public SKCanvas Canvas;
        public VisualState VisualState;

        public bool HasActiveAnimations { get; private set; }
        public void SetHasActiveAnimations() => HasActiveAnimations = true;

        public LayoutSurface(Device device, MeasureCache measureCache, SKCanvas canvas, VisualState visualState)
        {
            Device = device;
            MeasureCache = measureCache;
            Canvas = canvas;
            VisualState = visualState;
        }

        public virtual SKRect Paint(Widget widget, SKRect rect)
        {

            try
            {
                var result = widget.PaintInternal(this, rect);

#if DEBUGCONTAINER
            DebugRect(rect, SKColors.Gray.WithAlpha(64));
#endif

                if (widget is AnimatedWidget animation)
                    if (!animation.Animation.Completed)
                        HasActiveAnimations = true;

                return result;
            }
            catch (Exception ex)
            {
                throw new PaintException($"Unable to paint {widget}", ex);
            }
        }

        public virtual void ClipRect(SKRect cliprect)
        {
            if (Canvas == null) return;
            Canvas.Save();
            Canvas.ClipRect(cliprect);
        }

        public virtual void ResetRectClip()
        {
            if (Canvas == null) return;
            Canvas.Restore();
        }

        public virtual void ClipPath(SKPath clipPath)
        {
            if (Canvas == null) return;
            Canvas.Save();
            Canvas.ClipPath(clipPath);
        }

        public virtual void ResetPathClip()
        {
            if (Canvas == null) return;
            Canvas.Restore();
        }

        public void DebugRect(SKRect rect, SKColor color)
        {
            if (Canvas == null) return;
#if DEBUG
            using (var borderpaint = new SKPaint() { Color = color, IsStroke = true })
                Canvas.DrawRect(rect, borderpaint);
#endif
        }

        public void DebugGestureRect(SKRect rect, SKColor color)
        {
            if (Canvas == null) return;
#if DEBUG
            if (VisualState.ShowTouchRegions)
                using (var paint = new SKPaint() { Color = color })
                    Canvas.DrawRect(rect, paint);
#endif
        }

        public void DebugLine(float x1, float y1, float x2, float y2, SKColor color)
        {
            if (Canvas == null) return;
#if DEBUG
            using (var linepaint = new SKPaint() { Color = color, IsStroke = true })
                Canvas.DrawLine(x1, y1, x2, y2, linepaint);
#endif
        }

    }
}
