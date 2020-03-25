using FluidSharp.Engine;
using FluidSharp.Interop;
using FluidSharp.State;
using FluidSharp.Widgets;
using SkiaSharp;
using System;

#if __FORMS__
namespace FluidSharp.Views.Forms
#elif __WINDOWSFORMS__
using System.Windows.Forms;
namespace FluidSharp.Views.WindowsForms
#elif __ANDROID__
namespace FluidSharp.Views.Android
#elif __IOS__
namespace FluidSharp.Views.iOS
#elif __UWP__
namespace FluidSharp.Views.UWP
#endif
{
    public class FluidWidgetView : SkiaView, IFluidWidgetView
    {

        /// <summary>
        /// The source of widgets, either overwrite MakeWidget, or set the WidgetSource to implement a custom view
        /// </summary>
        public IWidgetSource WidgetSource { get => widgetSource; set { widgetSource = value; InvalidatePaint(); } }
        private IWidgetSource widgetSource;

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

        /// <summary>
        /// Either overwrite this method or set WidgetSource to implement a custom view
        /// </summary>
        public virtual Widget MakeWidget(VisualState visualState)
        {
            if (WidgetSource == null)
                return Rectangle.Fill(SKColors.Teal);
            else
                return WidgetSource.MakeWidget(visualState);
        }

        public void SetHeight(float height)
        {
            // Maximum height: screen / device size
#if __FORMS__
#elif __WINDOWSFORMS__
            height = Math.Min(Height, Screen.FromControl(this).WorkingArea.Height);
#elif __ANDROID__
#elif __IOS__
#elif __UWP__
#endif

            // Minimum height: 10
            height = Math.Max(height, 10);

            // Apply height
#if __FORMS__
#elif __WINDOWSFORMS__
            this.Height = (int)height;
#elif __ANDROID__
#elif __IOS__
#elif __UWP__
#endif
        }

#if __WINDOWSFORMS__
        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            if (disposing)
                Implementation.Dispose();
        }
#endif

    }
}
