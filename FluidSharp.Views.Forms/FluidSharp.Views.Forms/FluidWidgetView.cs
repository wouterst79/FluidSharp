using FluidSharp.Engine;
using FluidSharp.Interop;
using FluidSharp.State;
using FluidSharp.Views.Forms;
using FluidSharp.Widgets;
using System;

namespace FluidSharp.Views.Forms
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
            //Height = Math.Min(Height, Screen.FromControl(this).WorkingArea.Height);
            Height = Math.Max(Height, 10);
            //this.siz.Height = (int)Height;
        }

//        protected override void Dispose(bool disposing)
        //{
        //    base.Dispose(disposing);
        //    if (disposing)
        //        Implementation.Dispose();
        //}

    }
}
