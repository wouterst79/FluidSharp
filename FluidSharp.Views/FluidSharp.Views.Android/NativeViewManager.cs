#if DEBUG
#define PRINTEVENTS
#endif
using Android.App;
using Android.Views;
using Android.Widget;
using FluidSharp.Interop;
using FluidSharp.Widgets.Native;
using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FluidSharp.Views.Android
{

    public class NativeViewManager : NativeViewManagerBase<View>
    {

        //        public View View;
        public RelativeLayout ViewGroup;
        public Activity Activity;

        public NativeViewManager(RelativeLayout viewGroup)
        {
            ViewGroup = viewGroup;
            Activity = (Activity)viewGroup.Context;
        }

        public override IEnumerable<View> GetChildren()
        {
            //          if (View is ViewGroup group)
            for (int i = 0; i < ViewGroup.ChildCount; i++)
            {
                var child = ViewGroup.GetChildAt(i);
                if (!(child is SkiaView))
                    yield return child;
            }
        }
        public override SKSize GetControlSize(View control) => new SKSize((float)control.Width, (float)control.Height);

        public override void RegisterNewControl(View newControl)
        {
            Activity.RunOnUiThread(() =>
            {
                ViewGroup.AddView(newControl);
            });
        }

        public override void SetControlVisible(View control, bool visible)
        {
            Activity.RunOnUiThread(() =>
            {
                var visibility = visible ? ViewStates.Visible : ViewStates.Gone;
                if (control.Visibility != visibility)
                {
#if PRINTEVENTS
                    Debug.WriteLine($"setting hidden: {!visible}");
#endif

                    control.Visibility = visibility;
                    //if (control.IsFirstResponder)
                    //  control.ResignFirstResponder();

                    //                control.Visibility = visibility;

                }
            });
        }

        public override void UpdateControl(View control, NativeViewWidget nativeViewWidget, SKRect rect, SKRect original)
        {

            Activity.RunOnUiThread(() =>
            {

                var l = (int)rect.Left;
                var w = (int)rect.Width;
                var t = (int)rect.Top;
                var h = (int)rect.Height;
                if (!(control.LayoutParameters is RelativeLayout.LayoutParams existing) || existing.LeftMargin != l && existing.TopMargin != t || existing.Width != w || existing.Height != h)
                {

                    var parameters = new RelativeLayout.LayoutParams(w, h);
                    parameters.LeftMargin = l;
                    parameters.TopMargin = t;
                    control.LayoutParameters = parameters;

#if PRINTEVENTS
                    Debug.WriteLine($"setting frame: {control.Tag}");
                    //Debug.WriteLine($"setting frame: {bounds} ({control.Bounds}) {control.Tag}");
#endif
                }

                if (control is INativeViewImpl nativeImpl)
                    nativeImpl.UpdateControl(nativeViewWidget, rect, original);
            
            });

        }
    }

}
