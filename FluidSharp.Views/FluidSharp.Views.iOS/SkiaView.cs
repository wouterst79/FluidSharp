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

            var touchrecognizer = new TouchRecognizer(this);
            GestureRecognizers = new UIGestureRecognizer[] { touchrecognizer };
            touchrecognizer.Touch += Touchrecognizer_Touch;

            this.PaintSurface += SkiaControl_PaintSurface;
        }

        private void SkiaControl_PaintSurface(object sender, SKPaintSurfaceEventArgs e)
        {

            var canvas = e.Surface.Canvas;

            // Make sure the canvas is drawn using pixel coordinates (but still high res):
            var factor = (float)Math.Round(e.Info.Width / Width * 4) / 4;
            var platformzoom = SKMatrix.MakeScale(factor, factor);
            canvas.Concat(ref platformzoom);

            PaintViewSurface?.Invoke(this, new PaintSurfaceEventArgs(canvas, Width, Height, e.Surface, e.Info));

        }

        private void Touchrecognizer_Touch(object sender, TouchActionEventArgs e)
        {
            //System.Diagnostics.Debug.WriteLine("touch");
            Touch?.Invoke(this, e);
        }

        //protected override void Dispose(bool disposing)
        //{
        //    // detach all events before disposing
        //    var controller = (ISKCanvasViewController)Element;
        //    if (controller != null)
        //    {
        //        controller.SurfaceInvalidated -= OnSurfaceInvalidated;
        //        controller.GetCanvasSize -= OnGetCanvasSize;
        //    }

        //    var control = Control;
        //    if (control != null)
        //    {
        //        control.PaintSurface -= OnPaintSurface;
        //    }

        //    // detach, regardless of state
        //    touchHandler.Detach(control);

        //    base.Dispose(disposing);
        //}


    }
}
