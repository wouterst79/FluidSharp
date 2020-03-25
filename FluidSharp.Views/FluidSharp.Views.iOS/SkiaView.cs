using FluidSharp.Touch;
using SkiaSharp;
using SkiaSharp.Views.iOS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UIKit;

namespace FluidSharp.Views.iOS
{
    public class SkiaView : SKCanvasView, ISkiaView
    {


        public float Width => (float)Bounds.Width;
        public float Height => (float)Bounds.Height;

        public event EventHandler<PaintSurfaceEventArgs> PaintViewSurface;
        public event EventHandler<TouchActionEventArgs> Touch;

        public void InvalidatePaint()
        {
            InvokeOnMainThread(() =>
            {
                SetNeedsDisplay();
            });
        }

        public SkiaView()
        {

            this.IgnorePixelScaling = true;

            var touchrecognizer = new TouchRecognizer(this);
            GestureRecognizers = new UIGestureRecognizer[] { touchrecognizer };
            touchrecognizer.Touch += Touchrecognizer_Touch;

            this.PaintSurface += SkiaControl_PaintSurface;
        }

        private void SkiaControl_PaintSurface(object sender, SKPaintSurfaceEventArgs e)
        {
            PaintViewSurface?.Invoke(this, new PaintSurfaceEventArgs(e.Surface.Canvas, Width, Height, e.Surface, e.Info));
        }

        private void Touchrecognizer_Touch(object sender, TouchActionEventArgs e)
        {
            //System.Diagnostics.Debug.WriteLine("touch");
            Touch?.Invoke(this, e);
        }


    }
}
