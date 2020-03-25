#if DEBUG
//#define PRINTEVENTS
#endif
using FluidSharp.Interop;
using FluidSharp.Widgets.Native;
using SkiaSharp;
using System;
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

        public Control Control;

        public NativeViewManager(Control control)
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

        public override void UpdateControl(Control control, NativeViewWidget nativeViewWidget, SKRect rect)
        {

            // set child bounds
            var targetbounds = new Rectangle((int)rect.Left, (int)rect.Top, (int)rect.Width, (int)rect.Height);
            if (control.Bounds != targetbounds)
#if PRINTEVENTS
                Debug.WriteLine($"setting bounds: {targetbounds}");
#endif
                control.SetBounds(targetbounds.X, targetbounds.Y, targetbounds.Width, targetbounds.Height);

        }

        public override void SetControlVisible(Control control, bool visible)
        {
            if (control.Visible != visible)
#if PRINTEVENTS
                    Debug.WriteLine($"setting visible");
#endif
                control.Visible = visible;
        }

    }

}
