using System;
using FluidSharp.Touch;
using SkiaSharp;
using SkiaSharp.Views.Desktop;
using System.Drawing;

namespace FluidSharp.Views.WindowsForms
{

    public class SkiaView : SKControl, ISkiaView
    {

        float ISkiaView.Width => Width;
        float ISkiaView.Height => Height;

        public event EventHandler<PaintSurfaceEventArgs> PaintViewSurface;
        public event EventHandler<TouchActionEventArgs> Touch;

        public void InvalidatePaint()
        {
            this.Invalidate();
        }

        public SkiaView()
        {

            //this.IgnorePixelScaling = true; <== not supported on windows form

            this.PaintSurface += CanvasView_PaintSurface;

            this.MouseDown += new System.Windows.Forms.MouseEventHandler(SkiaControl_MouseDown);
            this.MouseMove += new System.Windows.Forms.MouseEventHandler(SkiaControl_MouseMove);
            this.MouseUp += new System.Windows.Forms.MouseEventHandler(SkiaControl_MouseUp);

        }

        private void CanvasView_PaintSurface(object sender, SKPaintSurfaceEventArgs e)
        {
            PaintViewSurface?.Invoke(this, new PaintSurfaceEventArgs(e.Surface.Canvas, Width, Height, e.Surface, e.Info));
        }

        private void SkiaControl_MouseDown(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            var onscreen = PointToScreen(new Point(e.X, e.Y));
            Touch.Invoke(this, new TouchActionEventArgs(0, TouchActionType.Pressed, new SKPoint(onscreen.X, onscreen.Y), new SKPoint(e.X, e.Y), true));
        }

        private void SkiaControl_MouseUp(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            var onscreen = PointToScreen(new Point(e.X, e.Y));
            Touch.Invoke(this, new TouchActionEventArgs(0, TouchActionType.Released, new SKPoint(onscreen.X, onscreen.Y), new SKPoint(e.X, e.Y), false));
        }

        private void SkiaControl_MouseMove(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            var onscreen = PointToScreen(new Point(e.X, e.Y));
            Touch.Invoke(this, new TouchActionEventArgs(0, TouchActionType.Moved, new SKPoint(onscreen.X, onscreen.Y), new SKPoint(e.X, e.Y), e.Button != System.Windows.Forms.MouseButtons.None));
        }

    }

}
