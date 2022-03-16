#define USEGL
using System;
using FluidSharp.Touch;
using SkiaSharp;
using SkiaSharp.Views.Desktop;
using System.Drawing;
using OpenTK;
using System.Windows.Forms;

namespace FluidSharp.Views.WindowsForms
{

#if USEGL
    public class SkiaView : SKGLControl, ISkiaView
#else
    public class SkiaView : SKControl, ISkiaView
#endif
    {

        float ISkiaView.Width => Width / PlatformScale.Width;
        float ISkiaView.Height => Height / PlatformScale.Height;

        public SKSize PlatformScale { get; set; }

        SKSize GetSize() => new SKSize(Width / PlatformScale.Width, Height / PlatformScale.Height);

        private bool autoScale = true;
        public bool AutoScale
        {
            get => autoScale; set
            {
                autoScale = value;
                if (!autoScale) PlatformScale = new SKSize(1, 1);
            }
        }

#if !USEGL
        public bool VSync;
#endif

        public event EventHandler<PaintSurfaceEventArgs> PaintViewSurface;
        public event EventHandler<TouchActionEventArgs> Touch;

        public void InvalidatePaint()
        {
            this.Invalidate();
        }

        public SkiaView()
        {

            //this.IgnorePixelScaling = true; <== not supported on windows form

#if !USEGL
            this.PaintSurface += CanvasView_PaintSurface;
#endif

            this.MouseDown += new System.Windows.Forms.MouseEventHandler(SkiaControl_MouseDown);
            this.MouseMove += new System.Windows.Forms.MouseEventHandler(SkiaControl_MouseMove);
            this.MouseUp += new System.Windows.Forms.MouseEventHandler(SkiaControl_MouseUp);

        }

        protected override void ScaleControl(SizeF factor, BoundsSpecified specified)
        {
            if (AutoScale)
                PlatformScale = new SKSize(factor.Width, factor.Width);
            base.ScaleControl(factor, specified);
        }

#if USEGL
        protected override void OnPaintSurface(SKPaintGLSurfaceEventArgs e)
        {
            base.OnPaintSurface(e);

            var canvas = e.Surface.Canvas;
            // Make sure the canvas is drawn using pixel coordinates (but still high res):
            //var factor = (float)MathF.Round(e.BackendRenderTarget.Width / Width * 4) / 4;
            var platformzoom = SKMatrix.MakeScale(PlatformScale.Width, PlatformScale.Height);
            canvas.Concat(ref platformzoom);

            PaintViewSurface?.Invoke(this, new PaintSurfaceEventArgs(e.Surface.Canvas, Width / PlatformScale.Width, Height / PlatformScale.Height, e.Surface, default));
        }
#endif

        private void CanvasView_PaintSurface(object sender, SKPaintSurfaceEventArgs e)
        {

            var canvas = e.Surface.Canvas;
            // Make sure the canvas is drawn using pixel coordinates (but still high res):
            //var factor = (float)MathF.Round(e.Info.Width / Width * 4) / 4;
            var platformzoom = SKMatrix.MakeScale(PlatformScale.Width, PlatformScale.Height);
            canvas.Concat(ref platformzoom);

            PaintViewSurface?.Invoke(this, new PaintSurfaceEventArgs(e.Surface.Canvas, Width / PlatformScale.Width, Height / PlatformScale.Height, e.Surface, e.Info));
        }

        private void SkiaControl_MouseDown(object sender, MouseEventArgs e)
        {
            var oncontrol = new Point((int)(e.X / PlatformScale.Width), (int)(e.Y / PlatformScale.Height));
            var onscreen = PointToScreen(oncontrol);
            Touch.Invoke(this, new TouchActionEventArgs(0, TouchActionType.Pressed, new SKPoint(onscreen.X, onscreen.Y), new SKPoint(oncontrol.X, oncontrol.Y), GetSize(), true));
        }

        private void SkiaControl_MouseUp(object sender, MouseEventArgs e)
        {
            var oncontrol = new Point((int)(e.X / PlatformScale.Width), (int)(e.Y / PlatformScale.Height));
            var onscreen = PointToScreen(oncontrol);
            Touch.Invoke(this, new TouchActionEventArgs(0, TouchActionType.Released, new SKPoint(onscreen.X, onscreen.Y), new SKPoint(oncontrol.X, oncontrol.Y), GetSize(), false));
        }

        private void SkiaControl_MouseMove(object sender, MouseEventArgs e)
        {
            var oncontrol = new Point((int)(e.X / PlatformScale.Width), (int)(e.Y / PlatformScale.Height));
            var onscreen = PointToScreen(oncontrol);
            Touch.Invoke(this, new TouchActionEventArgs(0, TouchActionType.Moved, new SKPoint(onscreen.X, onscreen.Y), new SKPoint(oncontrol.X, oncontrol.Y), GetSize(), e.Button != MouseButtons.None));
        }

    }

}
