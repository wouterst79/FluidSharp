﻿using Android.App;
using FluidSharp.Layouts;
using FluidSharp.Views.Android;
using System;
using System.Threading.Tasks;

namespace FluidSharp.Views.Android
{
    public class FluidUIActivity : Activity
    {

        public readonly IWidgetSource WidgetSource;
        private AndroidFluidWidgetView FluidWidgetView;

        //private KeyboardTracker KeyboardTracker;
        //private nfloat OriginalInsetBottom;

        private Action<Margins> OnDeviceMarginsChanged;

        public FluidUIActivity(IWidgetSource widgetSource, Action<Margins> onDeviceMarginsChanged)
        {
            WidgetSource = widgetSource;
            OnDeviceMarginsChanged = onDeviceMarginsChanged;
            //KeyboardTracker = new KeyboardTracker(h =>
            //{
            //    if (h == 0)
            //    {
            //        AdditionalSafeAreaInsets = new UIEdgeInsets(0, 0, 0, 0);
            //    }
            //    else
            //    {
            //        if (AdditionalSafeAreaInsets.Bottom == 0)
            //            OriginalInsetBottom = View.SafeAreaInsets.Bottom;
            //        AdditionalSafeAreaInsets = new UIEdgeInsets(0, 0, h - OriginalInsetBottom, 0);
            //    }
            //    View.LayoutIfNeeded();
            //});
        }

        //public override void ViewDidLayoutSubviews() { if (FluidWidgetView != null) Task.Run(RequestRedraw); }

        public Task RequestRedraw() => FluidWidgetView?.VisualState.RequestRedraw() ?? Task.CompletedTask;

//        public override void LoadView()
//        {
//            FluidWidgetView = new FluidWidgetView();
//            FluidWidgetView.WidgetSource = WidgetSource;
//            View = FluidWidgetView;
//        }

//        public override void ViewWillAppear(bool animated) => KeyboardTracker.RegisterForKeyboardNotifications();
//        public override void ViewWillDisappear(bool animated) => KeyboardTracker.UnregisterForKeyboardNotifications();

//        public override void ViewDidLoad()
//        {
//            base.ViewDidLoad();
//            // Perform any additional setup after loading the view, typically from a nib.
//        }

//        public override void DidReceiveMemoryWarning()
//        {
//            base.DidReceiveMemoryWarning();
//            // Release any cached data, images, etc that aren't in use.
//        }

//        public override void ViewSafeAreaInsetsDidChange()
//        {
//            var sai = View.SafeAreaInsets;
//            OnDeviceMarginsChanged(new Margins((float)sai.Left, (float)sai.Top, (float)sai.Right, (float)sai.Bottom));
//            base.ViewSafeAreaInsetsDidChange();
//        }

//        public override void TraitCollectionDidChange(UITraitCollection previousTraitCollection)
//        {
//            base.TraitCollectionDidChange(previousTraitCollection);
////            View.TraitCollection.UserInterfaceStyle == UIUserInterfaceStyle.Dark
//        }
    }
}