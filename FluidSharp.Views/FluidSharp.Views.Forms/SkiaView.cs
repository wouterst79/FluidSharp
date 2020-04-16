using FluidSharp;
using FluidSharp.Touch;
using SkiaSharp;
using SkiaSharp.Views.Forms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xamarin.Forms;

namespace FluidSharp.Views.Forms
{

    public class SkiaView : AbsoluteLayout, ISkiaView
    {

        SKCanvasView canvasView;

        float ISkiaView.Width => (float)Width;
        float ISkiaView.Height => (float)Height;

        float ScaleFactor;

        public event EventHandler<PaintSurfaceEventArgs> PaintViewSurface;
        public event EventHandler<TouchActionEventArgs> Touch;

        public virtual void OnPaintException(Exception exception) => throw exception;
        public virtual void OnTouchException(Exception exception) => throw exception;


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

            Children.Add(canvasView);

        }

        public IEnumerable<View> GetViews() => Children.Where(c => c != canvasView);

        private void CanvasView_PaintSurface(object sender, SKPaintSurfaceEventArgs e)
        {

            try
            {

                var canvas = e.Surface.Canvas;

                // Make sure the canvas is drawn using pixel coordinates (but still high res):
                var factor = (float)(e.Info.Width / Width);
                var platformzoom = SKMatrix.MakeScale(factor, factor);
                canvas.Concat(ref platformzoom);

                // use the scale factor for xamarin form touch event transformation as well
                ScaleFactor = factor;

                PaintViewSurface?.Invoke(this, new PaintSurfaceEventArgs(canvas, (float)Width, (float)Height, e.Surface, e.Info));

            }
            catch (Exception ex)
            {
                OnPaintException(ex);
            }
        }

        private void CanvasView_Touch(object sender, SKTouchEventArgs e)
        {

            try
            {

                //// request additional events
                if (e.ActionType != SKTouchAction.Exited && e.ActionType != SKTouchAction.Cancelled && e.ActionType != SKTouchAction.Released)
                    e.Handled = true;

                // TODO: Location On Device for Xamarin Forms (if this turns out to be needed)
                var locationondevice = new SKPoint(e.Location.X / ScaleFactor, e.Location.Y / ScaleFactor);
                var locationinview = new SKPoint(e.Location.X / ScaleFactor, e.Location.Y / ScaleFactor);
                Touch?.Invoke(this, new TouchActionEventArgs(e.Id, (TouchActionType)e.ActionType, locationondevice, locationinview, e.InContact));

            }
            catch (Exception ex)
            {
                OnTouchException(ex);
            }

        }

        protected override void OnSizeAllocated(double width, double height)
        {
            base.OnSizeAllocated(width, height);
            SetLayoutBounds(canvasView, new Rectangle(0, 0, width, height));
        }
    }

}
