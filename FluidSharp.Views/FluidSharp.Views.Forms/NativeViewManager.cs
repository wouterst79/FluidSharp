using FluidSharp.Interop;
using FluidSharp.Widgets.Native;
using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace FluidSharp.Views.Forms
{

    public class NativeViewManager : NativeViewManagerBase<View>
    {

        public SkiaView View;

        public NativeViewManager(SkiaView view)
        {            
            View = view;
        }

        public override IEnumerable<View> GetChildren() => View.GetViews();

        public override SKSize GetControlSize(View control) => new SKSize((float)control.Bounds.Width, (float)control.Bounds.Height);

        public override void RegisterNewControl(View newControl) => View.Children.Add(newControl);

        public override void SetControlVisible(View control, bool visible)
        {
            if (control.IsVisible != visible) control.IsVisible = visible;
        }

        public override void UpdateControl(View control, NativeViewWidget nativeViewWidget, SKRect rect)
        {
            var bounds = new Rectangle(rect.Left, rect.Top, rect.Width, rect.Height);
            if (control.Bounds != bounds)
            {
                AbsoluteLayout.SetLayoutBounds(control, bounds);
                System.Diagnostics.Debug.WriteLine("setting bounds");
            }
        }
    }

}
