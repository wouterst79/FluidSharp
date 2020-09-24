#if DEBUG
//#define DEBUGCONTAINER
#endif
using FluidSharp.Engine;
using FluidSharp.Layouts;
using FluidSharp.State;
using FluidSharp.Widgets;
using SkiaSharp;
using SkiaSharp.TextBlocks.Enum;
using Svg;
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

        public FlowDirection FlowDirection => Device.FlowDirection;
        public bool IsRtl => FlowDirection == FlowDirection.RightToLeft;

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

        public void DebugMargin(SKRect inner, Margins margins, SKColor color)
        {
#if DEBUG

            var top = new SKRect(inner.Left, inner.Top - margins.Top, inner.Right, inner.Top);
            DebugSpacing(top, margins.Top.ToString(), color);

            var bottom = new SKRect(inner.Left, inner.Bottom, inner.Right, inner.Bottom + margins.Bottom);
            DebugSpacing(bottom, margins.Bottom.ToString(), color);
            //var left = inner.HorizontalAlign(new SKSize(margins.Near, inner.Height), flowDirection)

            var near = inner.HorizontalAlign(new SKSize(-margins.Near, inner.Height), HorizontalAlignment.Near, FlowDirection);
            DebugSpacing(near, margins.Near.ToString(), color);

            var far = inner.HorizontalAlign(new SKSize(-margins.Far, inner.Height), HorizontalAlignment.Far, FlowDirection);
            DebugSpacing(far, margins.Far.ToString(), color);


#endif
        }

        public void DebugSpacing(SKRect dest, string text, SKColor color)
        {

            if (Canvas == null) return;
#if DEBUG

            if (!VisualState.ShowSpacing) return;

            if (dest.Width == 0 || dest.Height == 0) return;

            using (var paint = new SKPaint() { Color = color.WithAlpha(32) })
                Canvas.DrawRect(dest, paint);

            using (var paint = new SKPaint() { Color = color.WithAlpha(64), IsStroke = true })
                Canvas.DrawRect(dest, paint);

            using (var textpaint = new SKPaint() { Color = color, IsAntialias = true })
                Canvas.DrawText(text, dest.MidX - 4, dest.Bottom - 2, textpaint);
#endif

        }

    }
}
