using FluidSharp.Engine;
using FluidSharp.Interop;
using FluidSharp.State;
using FluidSharp.Widgets;
using SkiaSharp.Views.Desktop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FluidSharp.Views.WindowsForms
{

    public abstract class FluidWidgetView : SkiaView, IFluidWidgetView
    {

        public FluidWidgetViewImplementation Implementation;

        public NativeViewManager NativeViewManager;

        public FluidWidgetView()
        {
            var device = new Device();
            NativeViewManager = new NativeViewManager(this);
            Implementation = new FluidWidgetViewImplementation(this, this, device);
        }

        public bool AutoSizeHeight { get; set; }

        public INativeViewManager GetNativeViewManager() => NativeViewManager;

        public abstract Widget MakeWidget(VisualState visualState);
        public void SetHeight(float Height)
        {
            Height = Math.Min(Height, Screen.FromControl(this).WorkingArea.Height);
            Height = Math.Max(Height, 10);
            this.Height = (int)Height;
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            if (disposing)
                Implementation.Dispose();
        }

    }
}
