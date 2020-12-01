using System;
using System.Collections.Generic;
using System.Linq;

using CoreGraphics;
using Foundation;
using UIKit;
using FluidSharp.Touch;
using SkiaSharp;

namespace FluidSharp.Views.iOS
{
    class TouchRecognizer : UIGestureRecognizer
    {

        UIView view;            // iOS UIView 

        bool capture = true;

        static Dictionary<UIView, TouchRecognizer> viewDictionary =
            new Dictionary<UIView, TouchRecognizer>();

        static Dictionary<long, TouchRecognizer> idToTouchDictionary =
            new Dictionary<long, TouchRecognizer>();

        public event EventHandler<TouchActionEventArgs> Touch;

        public TouchRecognizer(UIView view)
        {
            this.view = view;
            viewDictionary.Add(view, this);
        }

        public void Detach()
        {
            viewDictionary.Remove(view);
        }

        // touches = touches of interest; evt = all touches of type UITouch
        public override void TouchesBegan(NSSet touches, UIEvent evt)
        {
            base.TouchesBegan(touches, evt);

            foreach (var touch in touches.Cast<UITouch>())
            {
                var id = touch.Handle.ToInt64();
                FireEvent(this, id, TouchActionType.Pressed, touch, true);

                if (!idToTouchDictionary.ContainsKey(id))
                    idToTouchDictionary.Add(id, this);
            }
        }

        public override void TouchesMoved(NSSet touches, UIEvent evt)
        {
            base.TouchesMoved(touches, evt);

            foreach (var touch in touches.Cast<UITouch>())
            {
                var id = touch.Handle.ToInt64();

                if (capture)
                    FireEvent(this, id, TouchActionType.Moved, touch, true);
                else
                {
                    CheckForBoundaryHop(touch);

                    if (idToTouchDictionary[id] != null)
                        FireEvent(idToTouchDictionary[id], id, TouchActionType.Moved, touch, true);
                }
            }
        }

        public override void TouchesEnded(NSSet touches, UIEvent evt)
        {
            base.TouchesEnded(touches, evt);

            foreach (var touch in touches.Cast<UITouch>())
            {
                var id = touch.Handle.ToInt64();

                if (capture)
                    FireEvent(this, id, TouchActionType.Released, touch, false);
                else
                {
                    CheckForBoundaryHop(touch);

                    if (idToTouchDictionary[id] != null)
                        FireEvent(idToTouchDictionary[id], id, TouchActionType.Released, touch, false);
                }
                idToTouchDictionary.Remove(id);
            }
        }

        public override void TouchesCancelled(NSSet touches, UIEvent evt)
        {
            base.TouchesCancelled(touches, evt);

            foreach (var touch in touches.Cast<UITouch>())
            {
                var id = touch.Handle.ToInt64();

                if (capture)
                    FireEvent(this, id, TouchActionType.Cancelled, touch, false);
                else if (idToTouchDictionary[id] != null)
                    FireEvent(idToTouchDictionary[id], id, TouchActionType.Cancelled, touch, false);
                idToTouchDictionary.Remove(id);
            }
        }

        void CheckForBoundaryHop(UITouch touch)
        {
            var id = touch.Handle.ToInt64();

            // TODO: Might require converting to a List for multiple hits
            TouchRecognizer recognizerHit = null;

            foreach (var view in viewDictionary.Keys)
            {
                var location = touch.LocationInView(view);

                if (new CGRect(new CGPoint(), view.Frame.Size).Contains(location))
                    recognizerHit = viewDictionary[view];
            }
            if (recognizerHit != idToTouchDictionary[id])
            {
                if (idToTouchDictionary[id] != null)
                    FireEvent(idToTouchDictionary[id], id, TouchActionType.Exited, touch, true);
                if (recognizerHit != null)
                    FireEvent(recognizerHit, id, TouchActionType.Entered, touch, true);
                idToTouchDictionary[id] = recognizerHit;
            }
        }

        void FireEvent(TouchRecognizer recognizer, long id, TouchActionType actionType, UITouch touch, bool isInContact)
        {

            var rootview = UIApplication.SharedApplication.KeyWindow.RootViewController.View;
            var ondevice = touch.LocationInView(rootview);
            var pointondevice = new SKPoint((float)ondevice.X, (float)ondevice.Y);

            // Convert touch location to Xamarin.Forms Point value
            var cgPoint = touch.LocationInView(recognizer.View);
            var xfPoint = new SKPoint((float)cgPoint.X, (float)cgPoint.Y);

            var viewsize = recognizer.View.Bounds.Size;
//            var cgsize = touch.LocationInView(recognizer.View);
            var size = new SKSize((float)viewsize.Width, (float)viewsize.Height);


            System.Diagnostics.Debug.WriteLine($"Touch {actionType} {pointondevice}");


            Touch?.Invoke(this, new TouchActionEventArgs(id, actionType, pointondevice, xfPoint, size, isInContact));

        }

        public override bool ShouldReceive(UIEvent @event)
        {
            var basevalue = base.ShouldReceive(@event);
            System.Diagnostics.Debug.WriteLine($"ShouldReceive {basevalue}");
            return true;
        }

        public override bool ShouldRequireFailureOfGestureRecognizer(UIGestureRecognizer otherGestureRecognizer)
        {
            var basevalue = base.ShouldRequireFailureOfGestureRecognizer(otherGestureRecognizer);
            System.Diagnostics.Debug.WriteLine($"ShouldRequireFailureOfGestureRecognizer {basevalue} - {otherGestureRecognizer}");
            return basevalue;
        }

        public override bool ShouldBeRequiredToFailByGestureRecognizer(UIGestureRecognizer otherGestureRecognizer)
        {
            var basevalue = base.ShouldBeRequiredToFailByGestureRecognizer(otherGestureRecognizer);
            System.Diagnostics.Debug.WriteLine($"ShouldBeRequiredToFailByGestureRecognizer {basevalue} - {otherGestureRecognizer}");
            return basevalue;
        }
    }
}