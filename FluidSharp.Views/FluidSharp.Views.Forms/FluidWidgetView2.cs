using FluidSharp.Engine;
using FluidSharp.Interop;
using FluidSharp.State;
using FluidSharp.Widgets;
using FluidSharp.Widgets.Native;
using SkiaSharp;
using System;

using FluidSharp.Views.Forms.NativeViews;
namespace FluidSharp.Views.Forms
{

    public class FluidWidgetView : SkiaView, IFluidWidgetView
    {

        /// <summary>
        /// The source of widgets, either overwrite MakeWidget, or set the WidgetSource to implement a custom view
        /// </summary>
        public IWidgetSource WidgetSource { get => widgetSource; set { widgetSource = value; SkiaView.InvalidatePaint(); } }
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


        private SkiaView SkiaView => this;

        public FluidWidgetView()
        {

            Device = new Device();

            Device.FlowDirection = Xamarin.Forms.Device.FlowDirection == Xamarin.Forms.FlowDirection.RightToLeft ? SkiaSharp.TextBlocks.Enum.FlowDirection.RightToLeft : SkiaSharp.TextBlocks.Enum.FlowDirection.LeftToRight;

            NativeViewManager = new NativeViewManager(this);

            Implementation = new FluidWidgetViewImplementation(SkiaView, this, Device);

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

            // Minimum height: 10
            height = Math.Max(height, 10);

            // Apply height
            if (this.Height < height - 5 || this.Height > height + 5) InvalidateMeasure();
        }


        public SKColor BackgroundColor
        {
            get
            {
                if (widgetSource is IBackgroundColorSource backgroundColorSource) return backgroundColorSource.BackgroundColor;
                return default;
            }
        }

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

    }
}

