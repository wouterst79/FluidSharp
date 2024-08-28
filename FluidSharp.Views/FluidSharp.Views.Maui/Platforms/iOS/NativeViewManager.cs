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
        public ISkiaView SkiaView;

        public NativeViewManager(UIView view, ISkiaView skiaView)
        {
            View = view;
            SkiaView = skiaView;
        }

        public override IEnumerable<UIView> GetChildren() => View.Subviews;

        public override SKSize GetControlSize(UIView control)
        {
            var scale = SkiaView.PlatformScale;
            return new SKSize((float)control.Bounds.Width / scale.Width, (float)control.Bounds.Height / scale.Height);
        }

        public override void RegisterNewControl(UIView newControl)
        {
            if (View.Subviews.Contains(newControl)) return;
            View.Add(newControl);
        }

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
            var scale = SkiaView.PlatformScale;

            var bounds = new CGRect(rect.Left * scale.Width, rect.Top * scale.Height, rect.Width * scale.Width, rect.Height * scale.Height);
            if (control.Frame != bounds)
            {
                control.Frame = bounds;
#if PRINTEVENTS
                Debug.WriteLine($"setting frame: {bounds} ({control.Bounds}) {control.Tag}");
#endif
            }

            if (control is INativeViewImpl nativeImpl)
            {
                var scaledrect = SKRect.Create(rect.Left * scale.Width, rect.Top * scale.Height, rect.Width * scale.Width, rect.Height * scale.Height);
                nativeImpl.UpdateControl(nativeViewWidget, scaledrect, original);
            }

        }
    }

}
