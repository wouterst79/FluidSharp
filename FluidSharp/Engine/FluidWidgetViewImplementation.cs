#if DEBUG
#endif

//#define PAINTPERF
//#define PAINTFPS

using FluidSharp.Layouts;
using FluidSharp.State;
using FluidSharp.Touch;
using FluidSharp.Widgets;
using FluidSharp.Widgets.Debugging;
using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace FluidSharp.Engine
{

    public class FluidWidgetViewImplementation : IDisposable
    {

        public ISkiaView View;
        public IFluidWidgetView WidgetView;

        private Device Device;
        private MeasureCache MeasureCache;

        public VisualState VisualState;
        private GestureArena GestureArena;

        public bool IsTransparent;

        public PerformanceTracker PerformanceTracker;

        private bool animationRunning;
        private bool painting;
        private bool AnimationRunning
        {
            get => animationRunning; set
            {
                if (animationRunning != value)
                {
                    //System.Diagnostics.Debug.WriteLine($"animation running set to {value}");
                    animationRunning = value;
                    if (value) EnsureAnimationRunning();
                }
            }
        }
        private Task AnimationDriverTask;

#if DEBUG
        public void ResetMeasureContext()
        {
            MeasureCache.Dispose();
            MeasureCache = new MeasureCache(Device, null, WidgetView.GetNativeViewManager());
            View.InvalidatePaint();
        }
#endif

        public FluidWidgetViewImplementation(ISkiaView skiaView, IFluidWidgetView widgetView, Device device)
        {

            Device = new Device();

            View = skiaView;
            WidgetView = widgetView;
            PerformanceTracker = new PerformanceTracker();
            VisualState = new VisualState(async () => View.InvalidatePaint(), PerformanceTracker);

            MeasureCache = new MeasureCache(Device, View.InvalidatePaint, WidgetView.GetNativeViewManager());

            skiaView.PaintViewSurface += View_PaintControlSurface;
            skiaView.Touch += View_Touch;

        }

        public FluidWidgetViewImplementation(ISkiaView skiaView, IFluidWidgetView widgetView, VisualState visualState, Device device)
        {

            Device = device;

            View = skiaView;
            WidgetView = widgetView;
            VisualState = visualState;
            PerformanceTracker = visualState.PerformanceTracker;

            MeasureCache = new MeasureCache(Device, View.InvalidatePaint, WidgetView.GetNativeViewManager());

            skiaView.PaintViewSurface += View_PaintControlSurface;
            skiaView.Touch += View_Touch;

        }

        #region IDisposable
        private bool disposed = false;

        public void Dispose()
        {
            Dispose(true);
        }
        ~FluidWidgetViewImplementation()
        {
            Dispose(false);
        }
        protected virtual void Dispose(bool disposing)
        {
            // Check to see if Dispose has already been called.
            if (!disposed)
            {
                //if (disposing)
                {
                    // Dispose managed resources.
                    MeasureCache.Dispose();
                    MeasureCache = null;
                }
                disposed = true;
            }
        }
        #endregion


        private Widget MakeWidget()
        {
            var controlwidget = WidgetView.MakeWidget(VisualState);
            if (Device.DefaultScale != 1)
                controlwidget = new Scale(Device.DefaultScale, controlwidget);

#if PAINTFPS || PAINTPERF

            controlwidget = new Container(ContainerLayout.Expand)
            {
                Children =
                {
                    controlwidget,
#if PAINTPERF
                    new PerformanceChart(),
#elif PAINTFPS
                    new FrameCounter(),
#endif
                }
            };
#endif

            return controlwidget;
        }

        private void View_PaintControlSurface(object sender, PaintSurfaceEventArgs e)
        {

            painting = true;

            PerformanceTracker.PaintStart();

            MeasureCache.NativeViewManager?.PaintStarted();

            var canvas = e.Canvas;
            canvas.Clear(IsTransparent ? SKColors.Transparent : SKColors.White);

            Widget widget;
            try
            {
                widget = MakeWidget();
            }
            catch (Exception ex)
            {
                throw new MakeWidgetException("unable to make widget", ex);
            }

            PerformanceTracker.WidgetsCreated();

            if (widget != null)
            {
                var width = e.Width;
                var height = e.Height;
                var layoutsurface = new LayoutSurface(Device, MeasureCache, canvas, VisualState);
                var actual = layoutsurface.Paint(widget, new SKRect(0, 0, width, height));

                if (WidgetView.AutoSizeHeight)
                    WidgetView.SetHeight(actual.Height);

                AnimationRunning = layoutsurface.HasActiveAnimations;

            }

            PerformanceTracker.PaintFinished();

            MeasureCache.NativeViewManager?.PaintCompleted();

            painting = false;

        }

        private void EnsureAnimationRunning()
        {
            if (AnimationDriverTask == null)
                AnimationDriverTask = AnimationDriver();
        }

        private async Task AnimationDriver()
        {
            while (AnimationRunning)
            {
                await Task.Delay(3);
                if (!painting)
                    View.InvalidatePaint();
            }
            AnimationDriverTask = null;

            PerformanceTracker.AnimationFinished();

        }

        public SKSize Measure(SKSize boundaries)
        {

            var widget = MakeWidget();
            if (widget == null) return boundaries;
            return widget.Measure(MeasureCache, boundaries);

        }

        private void View_Touch(object sender, TouchActionEventArgs e)
        {

#if DEBUG
            //Debug.WriteLine($"TOUCH: {e.PointerId}, {e.Type}: d={e.LocationOnDevice}, v={e.LocationInView}, {e.IsInContact}");
#endif

            if (e.IsInContact && GestureArena == null)
            {

                var widget = MakeWidget();

                if (widget == null)
                    return;

                var width = View.Width;
                var height = View.Height;

                var vs = new VisualState(async () => { }, null);
                var hittestlayout = new HitTestLayoutSurface(Device, MeasureCache, e.LocationInView, vs);
                hittestlayout.Paint(widget, new SKRect(0, 0, width, height));

                GestureArena = new GestureArena(hittestlayout.Hits, e.PointerId);

            }

            if (GestureArena != null)
            {
                GestureArena.Touch(e.PointerId, e.Type, e.LocationOnDevice, e.LocationInView, e.IsInContact, out var iscompleted);
                if (iscompleted) GestureArena = null;
            }

        }

    }
}
