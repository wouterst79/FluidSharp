using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Text;


// all credit to https://github.com/xamarin/xamarin-forms-samples/tree/master/Effects/TouchTrackingEffect/TouchTrackingEffect/TouchTrackingEffect


namespace FluidSharp.Touch
{
    public class TouchActionEventArgs
    {

        public long PointerId;
        public TouchActionType Type;
        public SKPoint LocationOnDevice;
        public SKPoint LocationInView;
        public SKSize ViewSize;
        public bool IsInContact;

        public TouchActionEventArgs(long id, TouchActionType type, SKPoint locationOnDevice, SKPoint locationInView, SKSize viewSize, bool isInContact)
        {
            PointerId = id;
            Type = type;
            LocationOnDevice = locationOnDevice;
            LocationInView = locationInView;
            ViewSize = viewSize;
            IsInContact = isInContact;
        }
    }
}
