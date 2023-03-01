#if DEBUG
#define DEBUGNEW
//#define DEBUGCONTAINER
#endif
using FluidSharp.Engine;
using FluidSharp.Layouts;
using FluidSharp.Paint;
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
        public SKCanvas Canvas { get; private set; }
        public virtual SKRect GetLocalClipRect() => Canvas?.LocalClipBounds ?? new SKRect();
        public VisualState VisualState;
        public bool AutoClip = true;

        public FlowDirection FlowDirection => Device.FlowDirection;
        public bool IsRtl => FlowDirection == FlowDirection.RightToLeft;

        public bool HasActiveAnimations { get; private set; }
        public void SetHasActiveAnimations() => HasActiveAnimations = true;

        public virtual void SetCanvas(SKCanvas canvas) { Canvas = canvas; }
        protected void SetThisCanvas(SKCanvas canvas) { Canvas = canvas; }

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

                var canvas = Canvas;
                if (canvas != null && AutoClip)
                {
                    if (rect.Bottom < 0 || rect.Top > GetLocalClipRect().Bottom)
                    {
                        // optimization: don't draw widgets if they are outside of clip rectangle. this appears to happen automatically for iOS, but not for Android
                        SetCanvas(null!);
                    }
                }

                var result = widget.PaintInternal(this, rect);

                if (Canvas != canvas)
                {
                    // restore canvas
                    SetCanvas(canvas!);
                }

#if DEBUGNEW
                if (widget.IsNew)
                {
                    widget.IsNew = false;
                    DebugNewRect(rect, SKColors.Blue.WithAlpha(32));
                }
#endif
#if DEBUGCONTAINER
            DebugRect(rect, SKColors.Gray.WithAlpha(64));
#endif

                if (widget is AnimatedWidget animation)
                    if (!animation.Animation.Completed)
                        HasActiveAnimations = true;

                return result;
            }
            catch (PaintException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new PaintException($"Unable to paint {widget}", widget, ex);
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
            Canvas.DrawRect(rect, PaintCache.GetBorderPaint(color, 1));
        }

        public void DebugNewRect(SKRect rect, SKColor color)
        {
            if (Canvas == null) return;
            if (VisualState.ShowNewWidgets)
                Canvas.DrawRect(rect, PaintCache.GetBackgroundPaint(color));
        }

        public static void DebugRecordedRect(SKCanvas Canvas, SKRect rect, SKColor color)
        {
            if (Canvas == null) return;
            if (VisualState.ShowRecordedWidgets)
                Canvas.DrawRect(rect, PaintCache.GetBackgroundPaint(color));
        }

        public void DebugGestureRect(SKRect rect, SKColor color)
        {
            if (Canvas == null) return;
            if (VisualState.ShowTouchRegions)
                Canvas.DrawRect(rect, PaintCache.GetBackgroundPaint(color));
        }

        public void DebugLine(float x1, float y1, float x2, float y2, SKColor color)
        {
            if (Canvas == null) return;
            Canvas.DrawLine(x1, y1, x2, y2, PaintCache.GetBorderPaint(color, 1));
        }

        public void DebugMargin(SKRect inner, Margins margins, SKColor color)
        {

            var top = new SKRect(inner.Left, inner.Top - margins.Top, inner.Right, inner.Top);
            DebugSpacing(top, margins.Top, color);

            var bottom = new SKRect(inner.Left, inner.Bottom, inner.Right, inner.Bottom + margins.Bottom);
            DebugSpacing(bottom, margins.Bottom, color);
            //var left = inner.HorizontalAlign(new SKSize(margins.Near, inner.Height), flowDirection)

            var near = inner.HorizontalAlign(new SKSize(-margins.Near, inner.Height), HorizontalAlignment.Near, FlowDirection);
            DebugSpacing(near, margins.Near, color);

            var far = inner.HorizontalAlign(new SKSize(-margins.Far, inner.Height), HorizontalAlignment.Far, FlowDirection);
            DebugSpacing(far, margins.Far, color);

        }

        public void DebugSpacing(SKRect dest, float text, SKColor color)
        {

            if (Canvas == null) return;

            if (!VisualState.ShowSpacing) return;

            if (dest.Width == 0 || dest.Height == 0) return;

            Canvas.DrawRect(dest, PaintCache.GetBackgroundPaint(color.WithAlpha(32)));

            Canvas.DrawRect(dest, PaintCache.GetBorderPaint(color.WithAlpha(64), 1));

            Canvas.DrawText(text.ToString(), dest.MidX - 4, dest.Bottom - 2, PaintCache.GetBackgroundPaint(color));

        }

        public void DebugSpacing(SKRect dest, Func<string> text, SKColor color)
        {

            if (Canvas == null) return;

            if (!VisualState.ShowSpacing) return;

            if (dest.Width == 0 || dest.Height == 0) return;

            Canvas.DrawRect(dest, PaintCache.GetBackgroundPaint(color.WithAlpha(32)));

            Canvas.DrawRect(dest, PaintCache.GetBorderPaint(color.WithAlpha(64), 1));

            Canvas.DrawText(text(), dest.MidX - 4, dest.Bottom - 2, PaintCache.GetBackgroundPaint(color));

        }

    }
}
