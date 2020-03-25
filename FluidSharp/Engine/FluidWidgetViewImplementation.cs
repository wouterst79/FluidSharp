﻿#if DEBUG
//#define DEBUGPERF
#define ENSUREPAINTED
#endif

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

        private VisualState VisualState;
        private GestureArena GestureArena;

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
            MeasureCache = new MeasureCache(Device, WidgetView.GetNativeViewManager());
            View.InvalidatePaint();
        }
#endif

        public FluidWidgetViewImplementation(ISkiaView control, IFluidWidgetView widgetControl, Device device)
        {

            Device = new Device();

            View = control;
            WidgetView = widgetControl;
            VisualState = new VisualState(async () => View.InvalidatePaint());

            MeasureCache = new MeasureCache(Device, WidgetView.GetNativeViewManager());

            control.PaintViewSurface += Control_PaintControlSurface;
            control.Touch += Control_Touch;
        }

        public FluidWidgetViewImplementation(ISkiaView control, IFluidWidgetView widgetControl, VisualState visualState, Device device)
        {

            Device = device;

            View = control;
            WidgetView = widgetControl;
            VisualState = visualState;

            MeasureCache = new MeasureCache(Device, WidgetView.GetNativeViewManager());

            control.PaintViewSurface += Control_PaintControlSurface;
            control.Touch += Control_Touch;
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

#if PAINTFPS

            controlwidget = new Column()
            {
                Children =
                {
                    controlwidget,
                    new FrameCounter(),
                }
            };
#endif

            return controlwidget;
        }

        private void Control_PaintControlSurface(object sender, PaintSurfaceEventArgs e)
        {

#if DEBUGPERF
            var stopwatch = new Stopwatch();
            stopwatch.Start();
#endif

            painting = true;

            MeasureCache.NativeViewManager?.PaintStarted();

            var canvas = e.Canvas;
            canvas.Clear(SKColors.White);

            var widget = MakeWidget();

#if DEBUGPERF
            Debug.WriteLine($"widget assembled in {stopwatch.ElapsedMilliseconds}ms");
#endif

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

#if DEBUGPERF
            Debug.WriteLine($"paint completed in {stopwatch.ElapsedMilliseconds}ms");
#endif

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

            //System.Diagnostics.Debug.WriteLine($"animation driver terminated");

        }

        public SKSize Measure(SKSize boundaries)
        {

            var widget = MakeWidget();
            if (widget == null) return boundaries;
            return widget.Measure(MeasureCache, boundaries);

        }

        private void Control_Touch(object sender, TouchActionEventArgs e)
        {

#if DEBUG
            //Debug.WriteLine($"TOUCH: {e.Id}, {e.Type}: d={e.LocationOnDevice}, v={e.LocationInView}, {e.IsInContact}");
#endif

            if (e.IsInContact && GestureArena == null)
            {

                var widget = MakeWidget();

                var width = View.Width;
                var height = View.Height;

                var vs = new VisualState(async () => { });
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