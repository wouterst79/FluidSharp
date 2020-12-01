#if DEBUG
#define PRINTEVENTS
#endif
using CoreGraphics;
using FluidSharp.Interop;
using FluidSharp.Widgets.Native;
using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UIKit;

namespace FluidSharp.Views.iOS
{

    public class NativeViewManager : NativeViewManagerBase<UIView>
    {

        public UIView View;

        public NativeViewManager(UIView view)
        {
            View = view;
        }

        public override IEnumerable<UIView> GetChildren() => View.Subviews;

        public override SKSize GetControlSize(UIView control) => new SKSize((float)control.Bounds.Width, (float)control.Bounds.Height);

        public override void RegisterNewControl(UIView newControl) => View.Add(newControl);

        public override void SetControlVisible(UIView control, bool visible)
        {
            if (control.Hidden == visible)
            {
#if PRINTEVENTS
                Debug.WriteLine($"setting hidden: {!visible}");
#endif

                if (control.IsFirstResponder)
                    control.ResignFirstResponder();

                control.Hidden = !visible;

            }
        }

        public override void UpdateControl(UIView control, NativeViewWidget nativeViewWidget, SKRect rect, SKRect original)
        {
            var bounds = new CGRect(rect.Left, rect.Top, rect.Width, rect.Height);
            if (control.Frame != bounds)
            {
                control.Frame = bounds;
#if PRINTEVENTS
                Debug.WriteLine($"setting frame: {bounds} ({control.Bounds}) {control.Tag}");
#endif
            }

            if (control is INativeViewImpl nativeImpl)
                nativeImpl.UpdateControl(nativeViewWidget, rect, original);

        }
    }

}
