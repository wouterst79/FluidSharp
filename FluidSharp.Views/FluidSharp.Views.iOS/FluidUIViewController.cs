using CoreGraphics;
using FluidSharp.Views.iOS;
using Foundation;
using System;
using UIKit;

namespace FluidSharp.Views.iOS
{
    public class FluidUIViewController : UIViewController
    {

        public readonly IWidgetSource WidgetSource;
        private FluidWidgetView FluidWidgetView;

        public FluidUIViewController(IWidgetSource widgetSource)
        {
            WidgetSource = widgetSource;
        }

        //public FluidUIViewController(IntPtr handle) : base(handle)
        //{
        //}

        public override void LoadView()
        {
            FluidWidgetView = new FluidWidgetView();
            FluidWidgetView.WidgetSource = WidgetSource;
            View = FluidWidgetView;
        }

        public override void ViewDidLoad ()
        {
            base.ViewDidLoad ();
            // Perform any additional setup after loading the view, typically from a nib.
        }

        public override void DidReceiveMemoryWarning ()
        {
            base.DidReceiveMemoryWarning ();
            // Release any cached data, images, etc that aren't in use.
        }
    }
}