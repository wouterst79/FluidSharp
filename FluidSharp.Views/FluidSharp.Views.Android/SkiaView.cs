#define USEGL
//using Android.Graphics;
using Android.App;
using Android.Views;
using FluidSharp.Touch;
using SkiaSharp;
using SkiaSharp.Views.Android;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AView = Android.Views.View;

namespace FluidSharp.Views.Android
{
#if USEGL
    public class SkiaView : SKGLSurfaceView, ISkiaView
#else
    public class SkiaView : SKCanvasView, ISkiaView
#endif
    {

        float ISkiaView.Width => Width / PlatformScale.Width;
        float ISkiaView.Height => Height / PlatformScale.Height;
        public SKSize PlatformScale;

        SKSize GetSize() => new SKSize(Width / PlatformScale.Width, Height / PlatformScale.Height);
        SKPoint ScalePoint(SKPoint point) => new SKPoint(point.X / PlatformScale.Width, point.Y / PlatformScale.Height);

        public event EventHandler<PaintSurfaceEventArgs> PaintViewSurface;
        public new event EventHandler<TouchActionEventArgs> Touch;

        public void InvalidatePaint()
        {
            ((Activity)Context).RunOnUiThread(() => Invalidate());
        }

        public SkiaView(global::Android.Content.Context context) : base(context)
        {

            PlatformScale = new SKSize(Resources.DisplayMetrics.Xdpi / 140, Resources.DisplayMetrics.Ydpi / 140);


#if !USEGL
            this.PaintSurface += SkiaControl_PaintSurface;
#endif
        }


#if USEGL
        protected override void OnPaintSurface(SKPaintGLSurfaceEventArgs e)
#else
        private void SkiaControl_PaintSurface(object sender, SKPaintSurfaceEventArgs e)
#endif
        {

            var canvas = e.Surface.Canvas;
            // Make sure the canvas is drawn using pixel coordinates (but still high res):


#if USEGL
            //          var factor = (float)MathF.Round(e.BackendRenderTarget.Width / w * 4) / 4;
#else
//            var factor = (float)MathF.Round(e.Info.Width / w * 4) / 4;
#endif
            var platformzoom = SKMatrix.CreateScale(PlatformScale.Width, PlatformScale.Height);
            //var platformzoom = SKMatrix.CreateScale(factor, factor);
            canvas.Concat(ref platformzoom);

            var w = ((ISkiaView)this).Width;
            var h = ((ISkiaView)this).Height;

            PaintViewSurface?.Invoke(this, new PaintSurfaceEventArgs(canvas, w, h, e.Surface, default));

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

        AView view;
        bool capture;
        Func<double, double> fromPixels;
        int[] twoIntArray = new int[2];

        const bool Capture = true;

        public override bool OnTouchEvent(MotionEvent e)
        {
            //    return base.OnTouchEvent(e);
            //}

            //private void OnTouch(object sender, TouchEventArgs args)
            //{

            //    // Two object common to all the events
            //    var senderView = sender as global::AView;
            //    MotionEvent motionEvent = args.Event;

            var senderView = this;
            var motionEvent = e;

            // Get the pointer index
            int pointerIndex = motionEvent.ActionIndex;

            // Get the id that identifies a finger over the course of its progress
            int id = motionEvent.GetPointerId(pointerIndex);

            senderView.GetLocationOnScreen(twoIntArray);

            var pointinview = ScalePoint(new SKPoint(motionEvent.GetX(pointerIndex), motionEvent.GetY(pointerIndex)));
            var pointondevice = ScalePoint(new SKPoint(twoIntArray[0] + pointinview.X,
                                            twoIntArray[1] + pointinview.Y));


            // Use ActionMasked here rather than Action to reduce the number of possibilities
            switch (motionEvent.ActionMasked)
            {
                case MotionEventActions.Down:
                case MotionEventActions.PointerDown:
                    FireEvent(id, TouchActionType.Pressed, pointondevice, pointinview, true);

                    capture = Capture;
                    break;

                case MotionEventActions.Move:
                    // Multiple Move events are bundled, so handle them in a loop
                    for (pointerIndex = 0; pointerIndex < motionEvent.PointerCount; pointerIndex++)
                    {
                        id = motionEvent.GetPointerId(pointerIndex);

                        if (capture)
                        {
                            senderView.GetLocationOnScreen(twoIntArray);

                            pointinview = ScalePoint(new SKPoint(motionEvent.GetX(pointerIndex), motionEvent.GetY(pointerIndex)));
                            pointondevice = ScalePoint(new SKPoint(twoIntArray[0] + pointinview.X,
                                                            twoIntArray[1] + pointinview.Y));

                            FireEvent(id, TouchActionType.Moved, pointondevice, pointinview, true);
                        }
                        else
                        {
                            //CheckForBoundaryHop(id, pointondevice);
                            FireEvent(id, TouchActionType.Moved, pointondevice, pointinview, true);
                        }
                    }
                    break;

                case MotionEventActions.Up:
                case MotionEventActions.Pointer1Up:
                    if (capture)
                    {
                        FireEvent(id, TouchActionType.Released, pointondevice, pointinview, false);
                    }
                    else
                    {
                        //CheckForBoundaryHop(id, pointondevice);
                        FireEvent(id, TouchActionType.Released, pointondevice, pointinview, false);
                    }
                    break;

                case MotionEventActions.Cancel:
                    if (capture)
                    {
                        FireEvent(id, TouchActionType.Cancelled, pointondevice, pointinview, false);
                    }
                    else
                    {
                        FireEvent(id, TouchActionType.Cancelled, pointondevice, pointinview, false);
                    }
                    break;
            }

            void FireEvent(long id, TouchActionType actionType, SKPoint pointondevice, SKPoint pointinview, bool isInContact)
            {

                //var rootview = UIApplication.SharedApplication.KeyWindow.RootViewController.View;
                //var ondevice = touch.LocationInView(rootview);
                //var pointondevice = new SKPoint((float)ondevice.X, (float)ondevice.Y);

                // Convert touch location to Xamarin.Forms Point value
                //var cgPoint = touch.LocationInView(recognizer.View);
                //var xfPoint = new SKPoint((float)cgPoint.X, (float)cgPoint.Y);

                //var viewsize = recognizer.View.Bounds.Size;
                //            var cgsize = touch.LocationInView(recognizer.View);
                //var size = new SKSize(Width, Height);
                var size = GetSize();

                System.Diagnostics.Debug.WriteLine($"Touch {actionType} {pointondevice}");


                Touch?.Invoke(this, new TouchActionEventArgs(id, actionType, pointondevice, pointinview, size, isInContact));

            }

            return false;
        }

        //void CheckForBoundaryHop(int id, Point pointerLocation)
        //{
        //    TouchEffect touchEffectHit = null;

        //    foreach (AView view in viewDictionary.Keys)
        //    {
        //        // Get the view rectangle
        //        try
        //        {
        //            view.GetLocationOnScreen(twoIntArray);
        //        }
        //        catch // System.ObjectDisposedException: Cannot access a disposed object.
        //        {
        //            continue;
        //        }
        //        Rectangle viewRect = new Rectangle(twoIntArray[0], twoIntArray[1], view.Width, view.Height);

        //        if (viewRect.Contains(pointerLocation))
        //        {
        //            touchEffectHit = viewDictionary[view];
        //        }
        //    }

        //    if (touchEffectHit != idToEffectDictionary[id])
        //    {
        //        if (idToEffectDictionary[id] != null)
        //        {
        //            FireEvent(idToEffectDictionary[id], id, TouchActionType.Exited, pointerLocation, true);
        //        }
        //        if (touchEffectHit != null)
        //        {
        //            FireEvent(touchEffectHit, id, TouchActionType.Entered, pointerLocation, true);
        //        }
        //        idToEffectDictionary[id] = touchEffectHit;
        //    }
        //}

        //void FireEvent(TouchEffect touchEffect, int id, TouchActionType actionType, Point pointerLocation, bool isInContact)
        //{
        //    // Get the method to call for firing events
        //    Action<Element, TouchActionEventArgs> onTouchAction = touchEffect.libTouchEffect.OnTouchAction;

        //    // Get the location of the pointer within the view
        //    touchEffect.view.GetLocationOnScreen(twoIntArray);
        //    double x = pointerLocation.X - twoIntArray[0];
        //    double y = pointerLocation.Y - twoIntArray[1];
        //    Point point = new Point(fromPixels(x), fromPixels(y));

        //    // Call the method
        //    onTouchAction(touchEffect.formsElement,
        //        new TouchActionEventArgs(id, actionType, point, isInContact));
        //}


    }
}
