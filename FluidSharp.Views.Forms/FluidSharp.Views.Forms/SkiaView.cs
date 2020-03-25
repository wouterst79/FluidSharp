using FluidSharp;
using FluidSharp.Touch;
using SkiaSharp;
using SkiaSharp.Views.Forms;
using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;

namespace FluidSharp.Views.Forms
{

    public class SkiaView : AbsoluteLayout, ISkiaView
    {

        SKCanvasView canvasView;

        float ISkiaView.Width => (float)Width;
        float ISkiaView.Height => (float)Height;

        public event EventHandler<PaintSurfaceEventArgs> PaintViewSurface;
        public event EventHandler<TouchActionEventArgs> Touch;

        public void InvalidatePaint()
        {
            Xamarin.Forms.Device.BeginInvokeOnMainThread(() =>
                canvasView.InvalidateSurface()
            );
        }

        public SkiaView()
        {

            canvasView = new SKCanvasView();
            canvasView.PaintSurface += CanvasView_PaintSurface;
            canvasView.Touch += CanvasView_Touch;

            canvasView.EnableTouchEvents = true;
            canvasView.IgnorePixelScaling = true;

            Children.Add(canvasView);

        }

        private void CanvasView_PaintSurface(object sender, SKPaintSurfaceEventArgs e)
        {
            PaintViewSurface?.Invoke(this, new PaintSurfaceEventArgs(e.Surface.Canvas, (float)Width, (float)Height, e.Surface, e.Info));
        }

        private void CanvasView_Touch(object sender, SKTouchEventArgs e)
        {
            Touch?.Invoke(this, new TouchActionEventArgs(e.Id, (TouchActionType)e.ActionType, e.Location, e.Location, e.InContact));
        }

        protected override void OnSizeAllocated(double width, double height)
        {
            base.OnSizeAllocated(width, height);
            SetLayoutBounds(canvasView, new Rectangle(0, 0, width, height));
        }
    }

}
