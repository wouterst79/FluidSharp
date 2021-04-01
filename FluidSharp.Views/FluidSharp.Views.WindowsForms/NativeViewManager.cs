#if DEBUG
#define PRINTEVENTS
#endif
using FluidSharp.Interop;
using FluidSharp.Widgets.Native;
using SkiaSharp;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FluidSharp.Views.WindowsForms
{

    public class NativeViewManager : NativeViewManagerBase<Control>
    {

        public SkiaView Control;

        private ConcurrentDictionary<Control, Rectangle> LastBounds = new ConcurrentDictionary<Control, Rectangle>();

        public NativeViewManager(SkiaView control)
        {
            Control = control;
        }

        public override void RegisterNewControl(Control newControl) => Control.Controls.Add(newControl);

        public override IEnumerable<Control> GetChildren()
        {
            //#if DEBUG
            //            foreach (var child in Control.Controls.Cast<Control>())
            //                System.Diagnostics.Debug.WriteLine($"{child.Tag} {child.Visible} {child.Bounds}");
            //#endif
            return Control.Controls.Cast<Control>();
        }

        public override SKSize GetControlSize(Control control) => new SKSize(control.Width, control.Height);

        public override void UpdateControl(Control control, NativeViewWidget nativeViewWidget, SKRect rect, SKRect original)
        {

            // set child bounds
            var scale = Control.PlatformScale;
            var targetbounds = new Rectangle((int)(rect.Left * scale.Width), (int)(rect.Top * scale.Height), (int)(rect.Width * scale.Width), (int)(rect.Height * scale.Height));
            if (!LastBounds.TryGetValue(control, out var currentbounds) || currentbounds != targetbounds)
            //if (control.Bounds != targetbounds && LastBounds != targetbounds)
            {
#if PRINTEVENTS
                Debug.WriteLine($"setting bounds: {targetbounds} ({control.Bounds}) {control.Tag}");
#endif
                control.SetBounds(targetbounds.X, targetbounds.Y, targetbounds.Width, targetbounds.Height);
                LastBounds[control] = targetbounds;
            }

            if (control is INativeViewImpl nativeImpl)
                nativeImpl.UpdateControl(nativeViewWidget, rect, original);
        }

        public override void SetControlVisible(Control control, bool visible)
        {
            if (control.Visible != visible)
            {
#if PRINTEVENTS
                    Debug.WriteLine($"setting visible {visible}");
#endif
                control.Visible = visible;
            }
        }

    }

}
