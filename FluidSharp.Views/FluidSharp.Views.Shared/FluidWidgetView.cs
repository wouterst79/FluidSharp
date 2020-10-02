using FluidSharp.Engine;
using FluidSharp.Interop;
using FluidSharp.State;
using FluidSharp.Widgets;
using FluidSharp.Widgets.Native;
using SkiaSharp;
using System;

#if __FORMS__
using FluidSharp.Views.Forms.NativeViews;
namespace FluidSharp.Views.Forms
#elif __WINDOWSFORMS__
using FluidSharp.Views.WindowsForms.NativeViews;
using System.Windows.Forms;
namespace FluidSharp.Views.WindowsForms
#elif __ANDROID__
namespace FluidSharp.Views.Android
#elif __IOS__
using FluidSharp.Views.iOS.NativeViews;
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

        public Device Device;

        private FluidWidgetViewImplementation implementation;
        public FluidWidgetViewImplementation Implementation
        {
            get => implementation;
            set
            {
                if (implementation != null) implementation.Dispose();
                implementation = value;
            }
        }
        public NativeViewManager NativeViewManager;

        public VisualState VisualState => Implementation.VisualState;

        /// <summary>
        /// Set AutoSizeHeight to true if the view should be sized by the (painted) height of the widgets.
        /// The default is false.
        /// </summary>
        public bool AutoSizeHeight { get; set; }
        private float LastPaintWidth = -1;
        private float LastHeightRequest = -1;


        public FluidWidgetView()
        {

            Device = new Device();

#if __FORMS__
            device.FlowDirection = Xamarin.Forms.Device.FlowDirection == Xamarin.Forms.FlowDirection.RightToLeft ? SkiaSharp.TextBlocks.Enum.FlowDirection.RightToLeft : SkiaSharp.TextBlocks.Enum.FlowDirection.LeftToRight;
#endif

            NativeViewManager = new NativeViewManager(this);

            Implementation = new FluidWidgetViewImplementation(this, this, Device);

            RegisterNativeViews();

        }

        protected FluidWidgetView(bool CreatesOwnImplementation)
        {
            if (!CreatesOwnImplementation) throw new ArgumentOutOfRangeException(nameof(CreatesOwnImplementation));
        }

        protected void RegisterNativeViews()
        {
            NativeViewManager.RegisterNativeView<NativeTextboxWidget, NativeTextboxImpl>(
                (w, c) => c.Context.Equals(w.Context),
                (w) => new NativeTextboxImpl(VisualState.RequestRedraw) { Context = w.Context }
            );
        }

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
            if (this.Height < height - 5 || this.Height > height + 5) InvalidateMeasure();
#elif __WINDOWSFORMS__
            this.Height = (int)height;
#elif __ANDROID__
#elif __IOS__
#elif __UWP__
#endif
        }

        public virtual SKColor GetBackgroundColor(VisualState visualState)
        {
            if (widgetSource is IBackgroundColorSource backgroundColorSource) return backgroundColorSource.GetBackgroundColor(visualState);
            return default;
        }


#if __FORMS__

        protected override void OnSizeAllocated(double width, double height)
        {
            base.OnSizeAllocated(width, height);
            LastPaintWidth = (float)Width;
        }

        protected override void InvalidateMeasure()
        {
            base.InvalidateMeasure();
            InvalidatePaint();
        }

        protected override void OnBindingContextChanged()
        {
            base.OnBindingContextChanged();
            if (BindingContext == null)
                Implementation.Dispose();
        }

        protected override Xamarin.Forms.SizeRequest OnMeasure(double widthConstraint, double heightConstraint)
        {

            if (Width == LastPaintWidth && LastHeightRequest > -1)
                return new Xamarin.Forms.SizeRequest(new Xamarin.Forms.Size(LastPaintWidth, LastHeightRequest));

            var request = Implementation.Measure(new SKSize((float)widthConstraint, (float)heightConstraint));
            System.Diagnostics.Debug.WriteLine($"LinkTileView Measured: {request} ({widthConstraint}, {heightConstraint}) ");

            if (float.IsInfinity(request.Width) || float.IsInfinity(request.Height))
                return base.OnMeasure(widthConstraint, heightConstraint);

            return new Xamarin.Forms.SizeRequest(new Xamarin.Forms.Size(request.Width, request.Height));

        }

#endif

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
