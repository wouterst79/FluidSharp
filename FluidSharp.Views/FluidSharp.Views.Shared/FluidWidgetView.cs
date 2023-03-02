#if !__IOS__ && !__ANDROID__

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
using FluidSharp.Views.Android.NativeViews;
using Android.App;
using Android.Views;
namespace FluidSharp.Views.Android
#elif __IOS__
using UIKit;
using FluidSharp.Views.iOS.NativeViews;
namespace FluidSharp.Views.iOS
#elif __UWP__
namespace FluidSharp.Views.UWP
#endif
{

#if __ANDROID__
    public class FluidWidgetView : global::Android.Widget.RelativeLayout, IFluidWidgetView
#else
    public class FluidWidgetView : SkiaView, IFluidWidgetView
#endif
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

#if __ANDROID__

        private SkiaView SkiaView;
        public SKSize PlatformScale => SkiaView.PlatformScale;

        public FluidWidgetView(global::Android.Content.Context context) : base(context)
        {

            SkiaView = new SkiaView(context);
            var fillparams = new ViewGroup.LayoutParams(ViewGroup.LayoutParams.MatchParent, ViewGroup.LayoutParams.MatchParent);
            AddView(SkiaView, fillparams);

#else

        private SkiaView SkiaView => this;

        public FluidWidgetView()
        {
#endif

            Device = new Device();

#if __FORMS__
            Device.FlowDirection = Xamarin.Forms.Device.FlowDirection == Xamarin.Forms.FlowDirection.RightToLeft ? SkiaSharp.TextBlocks.Enum.FlowDirection.RightToLeft : SkiaSharp.TextBlocks.Enum.FlowDirection.LeftToRight;
#endif

            NativeViewManager = new NativeViewManager(this);

            Implementation = new FluidWidgetViewImplementation(SkiaView, this, Device);

            RegisterNativeViews();

        }

#if __ANDROID__
        public FluidWidgetView(global::Android.Content.Context context, bool CreatesOwnImplementation) : base(context)
#else
        protected FluidWidgetView(bool CreatesOwnImplementation)
#endif
        {
            if (!CreatesOwnImplementation) throw new ArgumentOutOfRangeException(nameof(CreatesOwnImplementation));
        }

        protected void RegisterNativeViews()
        {
#if __IOS__
            NativeViewManager.RegisterNativeView<NativeTextboxWidget, UIView>(
                (w, c) => c is INativeTextboxImpl impl && impl.Context.Equals(w.Context),
                (w) => w.Keyboard == Keyboard.MultiLine ? (UIView)
                    new NativeMultiLineTextboxImpl(VisualState.RequestRedraw) { Context = w.Context } :
                    new NativeSingleLineTextboxImpl(VisualState.RequestRedraw) { Context = w.Context }
#else
            NativeViewManager.RegisterNativeView<NativeTextboxWidget, NativeTextboxImpl>(
                (w, c) => c.Context.Equals(w.Context),
#if __ANDROID__
                (w) => new NativeTextboxImpl(Context, VisualState.RequestRedraw) { Context = w.Context }
#else
                (w) => new NativeTextboxImpl(VisualState.RequestRedraw) { Context = w.Context }
#endif
#endif
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


        public SKColor BackgroundColor
        {
            get
            {
                if (widgetSource is IBackgroundColorSource backgroundColorSource) return backgroundColorSource.BackgroundColor;
                return default;
            }
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

#if __ANDROID__
        public void AddOnMainThread(View childview)
        {

            ((Activity)Context).RunOnUiThread(() => AddView(childview));

        }
#endif

    }
}

#endif