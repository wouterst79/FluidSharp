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
        public AndroidFluidWidgetView ViewGroup;

        public NativeViewManager(AndroidFluidWidgetView viewGroup)
        {
            ViewGroup = viewGroup;
        }

        public override IEnumerable<View> GetChildren()
        {
            //          if (View is ViewGroup group)
            for (int i = 0; i < ViewGroup.ChildCount; i++)
            {
                var child = ViewGroup.GetChildAt(i);
                if (!(child is SkiaGLView || child is SkiaCanvasView))
                    yield return child;
            }
        }
        public override SKSize GetControlSize(View control)
        {
            var scale = ViewGroup.PlatformScale;
            return new SKSize(control.Width / scale.Width, control.Height / scale.Height);
        }

        public override void RegisterNewControl(View newControl)
        {
            ViewGroup.AddOnMainThread(newControl);
        }

        public override void SetControlVisible(View control, bool visible)
        {
            ((INativeViewImpl)control).SetVisible(visible);
        }

        public override void UpdateControl(View control, NativeViewWidget nativeViewWidget, SKRect rect, SKRect original)
        {

            if (control is INativeViewImpl nativeImpl)
            {
                var scale = ViewGroup.PlatformScale;
                var targetbounds = new SKRect((int)(rect.Left * scale.Width), (int)(rect.Top * scale.Height), (int)(rect.Width * scale.Width), (int)(rect.Height * scale.Height));

                nativeImpl.SetBounds(targetbounds);
                nativeImpl.UpdateControl(nativeViewWidget, rect, original);

            }

        }
    }

}
