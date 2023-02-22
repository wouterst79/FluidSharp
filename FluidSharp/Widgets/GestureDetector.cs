using FluidSharp.Engine;
using FluidSharp.Layouts;
using FluidSharp.State;
using FluidSharp.Touch;
using SkiaSharp;
using SkiaSharp.TextBlocks.Enum;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace FluidSharp.Widgets
{

    public abstract class GestureDetector : Widget
    {

        public static EventHandler<TapHandlerException>? OnTapException;

        public object Context;

        public bool IsMultiTouch;
        public Func<SKPoint, Task> OnTouchDown;
        public Func<Task> OnWin;
        public Func<bool, TimeSpan, Task> OnTouchUp;

        public Widget Child;


        protected const float TapLooseThresholdSQ = 10f * 10f;
        protected const float PanWinThreshold = 10f;
        protected const int VelocityMinimumWindowMS = 30;


        #region struct Movement

        public struct Movement
        {

            public HitTestHit Hit;

            public SKPoint StartLocationOnDevice;
            public SKPoint CurrentLocationOnDevice;
            public SKPoint DeltaLocationOnDevice;

            public SKPoint StartLocationInView;
            public SKPoint CurrentLocationInView;
            public SKPoint DeltaLocationInView;

            public SKSize ViewSize;

            public DateTime StartTime;
            public DateTime CurrentTime;
            public TimeSpan DeltaTime;

            public bool HasWon;

            public bool IsInitialPointer;

            public Movement(HitTestHit hit, SKPoint startLocationOnDevice, SKPoint currentLocationOnDevice, SKPoint startLocationInView, SKPoint currentLocationInView, SKSize viewSize, DateTime startTime, DateTime currentTime, bool hasWon, bool isInitialPointer) : this()
            {

                Hit = hit;

                StartLocationOnDevice = startLocationOnDevice;
                CurrentLocationOnDevice = currentLocationOnDevice;
                DeltaLocationOnDevice = currentLocationOnDevice - startLocationOnDevice;

                StartLocationInView = startLocationInView;
                CurrentLocationInView = currentLocationInView;
                DeltaLocationInView = currentLocationInView - startLocationInView;

                ViewSize = viewSize;

                StartTime = startTime;
                CurrentTime = currentTime;
                DeltaTime = currentTime - startTime;

                HasWon = hasWon;

                IsInitialPointer = isInitialPointer;

            }

            //public Movement MakeIdentity()
            //{
            //    return new Movement(Hit, StartLocationOnDevice, StartLocationOnDevice, StartLocationInView, StartLocationInView, StartTime, CurrentTime, false);
            //}
        }

        #endregion


        public GestureDetector(VisualState visualState, object context, Widget child)
        {
            Context = context;
            Child = child ?? throw new ArgumentNullException(nameof(child));
        }

        public static TapGestureDetector TapDetector(VisualState visualState, object context, Func<Task> OnTapped, Widget child)
        {
            return new TapGestureDetector(visualState, context, OnTapped, null, child);
        }

        public static TapGestureDetector TapDetector(VisualState visualState, object context, Func<Task> OnTapped, Func<Task>? OnLongTapped, Widget child)
        {
            return new TapGestureDetector(visualState, context, OnTapped, OnLongTapped, child);
        }


        public static PanGestureDetector HorizontalPanDetector(VisualState visualState, object context, SlidingCellState cellstate, Widget child)
        {
            return new HorizontalPanGestureDetector(visualState, context, false, pan => cellstate.SetPan(context, pan.X, visualState), velocity => cellstate.EndPan(velocity, visualState), child);
        }

        public static PanGestureDetector HorizontalPanDetector<T>(VisualState visualState, CarouselState<T> stateTransition, Widget child)
        {
            return new HorizontalPanGestureDetector(visualState, stateTransition, true, pan => stateTransition.SetRelativePan(pan.X, visualState), velocity => stateTransition.EndPan(velocity, visualState), child);
        }

        public static PanGestureDetector HorizontalPanDetector<T>(VisualState visualState, CarouselState<T> stateTransition, float pandetectorwidth, Widget child)
        {
            return new HorizontalPanEdgeGestureDetector(visualState, pandetectorwidth, stateTransition, true, pan => stateTransition.SetRelativePan(pan.X, visualState), velocity => stateTransition.EndPan(velocity, visualState), child);
        }

        public static PanGestureDetector HorizontalPanDetector(VisualState visualState, ScrollState scrollState, Widget child)
        {
            return new HorizontalPanGestureDetector(visualState, scrollState, false, pan => scrollState.SetPan(pan.X, visualState), velocity => scrollState.EndPan(new SKPoint(velocity.Y, velocity.X), visualState), child); // endpan: swap coordinates (because of scrollState reuse)
        }

        public static PanGestureDetector VerticalPanDetector(VisualState visualState, ScrollState scrollState, Widget child)
        {
            return new VerticalPanGestureDetector(visualState, scrollState, false, pan => scrollState.SetPan(pan.Y, visualState), velocity => scrollState.EndPan(velocity, visualState), child);
        }



        public virtual void Pressed(SKPoint locationOnWidget)
        {
            if (OnTouchDown != null) Task.Run(() => OnTouchDown(locationOnWidget));
        }

        public abstract (bool win, bool loose) Move(Movement movement);

        public virtual void Released(TimeSpan duration)
        {
            if (OnTouchUp != null) Task.Run(() => OnTouchUp(false, duration));
        }

        public virtual void Cancelled(TimeSpan duration)
        {
            if (OnTouchUp != null) Task.Run(() => OnTouchUp(true, duration));
        }

        public void Invoke(Func<Task>? func)
        {
            if (func != null) Task.Run(func);
        }


        // Widget Implementation
        public override SKSize Measure(MeasureCache measureCache, SKSize boundaries) => Child.Measure(measureCache, boundaries);
        public override SKRect PaintInternal(LayoutSurface layoutsurface, SKRect rect)
        {
            var childrect = layoutsurface.Paint(Child, rect);
            layoutsurface.DebugGestureRect(childrect, SKColors.Red.WithAlpha(32));
            return childrect;
        }

        // Detectors
        public class TapGestureDetector : GestureDetector
        {

            public Func<Task> OnTapped;
            public Func<Task>? OnLongTapped;

            public static TimeSpan LongTapDuration = TimeSpan.FromMilliseconds(500);

            public static TimeSpan DefaultTapThrottle = TimeSpan.FromMilliseconds(500);

            public TimeSpan TapThrottle = DefaultTapThrottle;

            private static DateTime LastTapDetected;
            private static object? LastTapContext;

            public TapGestureDetector(VisualState visualState, object context, Func<Task> onTapped, Func<Task>? onLongTapped, Widget child) : base(visualState, context, child)
            {
                OnTouchDown = async (location) => visualState.TouchTarget = new TouchTarget(new TapContext(context), location);
                OnTouchUp = async (cancelled, duration) =>
                {

                    if (!cancelled)
                    {

                        if (LastTapDetected.Add(TapThrottle) < DateTime.Now || !visualState.TouchTarget.IsContext(LastTapContext))
                        {

                            LastTapContext = context;
                            LastTapDetected = DateTime.Now;

                            if (duration > LongTapDuration && OnLongTapped != null)
                                Invoke(OnLongTapped);
                            else
                                Invoke(OnTapped);

                        }
                    
                    }

                    visualState.TouchTarget = new TouchTarget();

                };
                OnTapped = async () =>
                {
                    try
                    {
                        await onTapped();
                    }
                    catch (Exception ex)
                    {
                        var tapex = new TapHandlerException("tap handler failed", context, ex);
                        if (OnTapException == null) throw tapex;
                        else OnTapException(null, tapex);
                    }
                    await visualState.RequestRedraw();
                };
                if (onLongTapped != null)
                {
                    OnLongTapped = async () =>
                    {
                        try
                        {
                            await onLongTapped();
                        }
                        catch (Exception ex)
                        {
                            var tapex = new TapHandlerException("long tap handler failed", context, ex);
                            if (OnTapException == null) throw tapex;
                            else OnTapException(null, tapex);
                        }
                        await visualState.RequestRedraw();
                    };
                }
            }

            public override (bool win, bool loose) Move(Movement movement)
            {
                var scale = movement.Hit.Scale;
                var devicemovement = new SKPoint(movement.DeltaLocationOnDevice.X / scale.X, movement.DeltaLocationOnDevice.Y / scale.Y);
                var widgetmovement = new SKPoint(movement.DeltaLocationInView.X / scale.X, movement.DeltaLocationInView.Y / scale.Y);

                //System.Diagnostics.Debug.WriteLine($"widgetmovement:{widgetmovement}");

                var loose = devicemovement.LengthSquared > TapLooseThresholdSQ
                         || widgetmovement.LengthSquared > TapLooseThresholdSQ;
                return (false, loose);
            }

        }

        // Detectors
        public class TouchLocationDetector : GestureDetector
        {

            public Func<SKPoint, Task> OnTouch;

            public TouchLocationDetector(VisualState visualState, object context, Func<SKPoint, Task> onTouch, Widget child) : base(visualState, context, child)
            {
                OnWin = async () => visualState.TouchTarget = new TouchTarget(new TapContext(context), default);
                OnTouchUp = async (cancelled, duration) => visualState.TouchTarget = new TouchTarget();
                OnTouch = onTouch;
            }

            public override void Pressed(SKPoint locationOnWidget)
            {
                base.Pressed(locationOnWidget);

                var move = new SKPoint(locationOnWidget.X, locationOnWidget.Y);
                if (OnTouch != null)
                    Task.Run(() => OnTouch(move));

                Task.Run(OnWin);

            }

            public override (bool win, bool loose) Move(Movement movement)
            {

                var hitlocation = movement.Hit.LocationInWidget;
                var scale = movement.Hit.Scale;
                var location = new SKPoint(hitlocation.X + movement.DeltaLocationInView.X / scale.X, hitlocation.Y + movement.DeltaLocationInView.Y / scale.Y);

                // invoke move
                var move = new SKPoint(location.X, location.Y);
                if (OnTouch != null)
                    Task.Run(() => OnTouch(move));

                return (true, false);
            }

        }


        public class HorizontalPanGestureDetector : PanGestureDetector
        {

            public HorizontalPanGestureDetector(VisualState visualState, object context, bool relativeMove, Func<SKPoint, Task> onPanMove, Func<SKPoint, Task> onPanEnd, Widget child) : base(visualState, context, relativeMove, onPanMove, onPanEnd, child) { }

            public override (bool win, bool loose) Move(Movement movement)
            {
                if (movement.HasWon)
                    return base.Move(movement);
                else
                {
                    //base.Move(movement.MakeIdentity()); // stop any animations that may be in progress (now doing that in onpressed instead)
                    var haswon = Math.Abs(movement.DeltaLocationInView.X) > PanWinThreshold;
                    return (haswon, false);
                }
            }
        }

        public class HorizontalPanEdgeGestureDetector : HorizontalPanGestureDetector
        {

            public float Width;

            public HorizontalPanEdgeGestureDetector(VisualState visualState, float width, object context, bool relativeMove, Func<SKPoint, Task> onPanMove, Func<SKPoint, Task> onPanEnd, Widget child) : base(visualState, context, relativeMove, onPanMove, onPanEnd, child)
            {
                Width = width;
            }

            public override SKRect PaintInternal(LayoutSurface layoutsurface, SKRect rect)
            {
                var childrect = layoutsurface.Paint(Child, rect);
                var detectorrect = rect.HorizontalAlign(new SKSize(Width, rect.Height), HorizontalAlignment.Near, layoutsurface.FlowDirection);
                layoutsurface.DebugGestureRect(detectorrect, SKColors.Purple.WithAlpha(32));
                return childrect;
            }

            public override (bool win, bool loose) Move(Movement movement)
            {
                var device = movement.Hit.Device;
                if (device.FlowDirection == FlowDirection.LeftToRight)
                    if (movement.StartLocationInView.X > Width)
                        return (false, true);

                if (device.FlowDirection == FlowDirection.RightToLeft)
                    if (movement.StartLocationInView.X < movement.ViewSize.Width - Width)
                        return (false, true);

                if (movement.HasWon)
                    return base.Move(movement);
                else
                {
                    //base.Move(movement.MakeIdentity()); // stop any animations that may be in progress (now doing that in onpressed instead)
                    var haswon = Math.Abs(movement.DeltaLocationInView.X) > PanWinThreshold;
                    return (haswon, false);
                }
            }
        }

        public class VerticalPanGestureDetector : PanGestureDetector
        {

            public VerticalPanGestureDetector(VisualState visualState, object context, bool relativeMove, Func<SKPoint, Task> onPanMove, Func<SKPoint, Task> onPanEnd, Widget child) : base(visualState, context, relativeMove, onPanMove, onPanEnd, child) { }

            public override (bool win, bool loose) Move(Movement movement)
            {
                if (movement.HasWon)
                {
                    return base.Move(movement);
                }
                else
                {
                    //base.Move(movement.MakeIdentity()); // stop any animations that may be in progress (now doing that in onpressed instead)
                    var haswon = Math.Abs(movement.DeltaLocationInView.Y) > PanWinThreshold;
                    return (haswon, false);
                }
            }
        }

        // Detectors
        public class PanGestureDetector : GestureDetector
        {

            public bool RelativeMove;
            public Func<SKPoint, Task> OnPanMove;
            public Func<SKPoint, Task> OnPanEnd;

            public Movement LastVelocityMovement;
            public SKPoint Velocity; // points per second

            public PanGestureDetector(VisualState visualState, object context, bool relativeMove, Func<SKPoint, Task> onPanMove, Func<SKPoint, Task> onPanEnd, Widget child) : base(visualState, context, child)
            {
                RelativeMove = relativeMove;
                OnWin = async () => visualState.TouchTarget = new TouchTarget(new PanContext(context), default);
                OnPanMove = onPanMove;
                OnPanEnd = onPanEnd;
            }

            public override void Pressed(SKPoint locationOnWidget)
            {
                base.Pressed(locationOnWidget);

                var move = new SKPoint();
                if (OnPanMove != null)
                    Task.Run(() => OnPanMove(move));

            }

            public override (bool win, bool loose) Move(Movement movement)
            {

                var device = movement.Hit.Device;

                // calculate velocity
                if (LastVelocityMovement.StartTime == default)
                    LastVelocityMovement = movement;
                else if (LastVelocityMovement.CurrentTime.AddMilliseconds(VelocityMinimumWindowMS) <= movement.CurrentTime)
                {

                    var ms = (float)(movement.CurrentTime - LastVelocityMovement.CurrentTime).TotalMilliseconds;
                    var delta = movement.CurrentLocationInView - LastVelocityMovement.CurrentLocationInView;
                    var scale = movement.Hit.Scale;
                    var scaled = new SKPoint(delta.X / scale.X, delta.Y / scale.Y);
                    Velocity = new SKPoint(scaled.X * 1000 / ms, scaled.Y * 1000 / ms);

                    if (device.FlowDirection == FlowDirection.RightToLeft)
                        Velocity.X = -Velocity.X;

                    //System.Diagnostics.Debug.WriteLine($"pan velocity: {Velocity}");

                    LastVelocityMovement = movement;
                }

                // invoke move
                if (OnPanMove != null)
                {

                    var move = movement.DeltaLocationInView;

                    if (RelativeMove)
                    {
                        var pctX = movement.DeltaLocationInView.X / movement.Hit.WidgetRect.Width / movement.Hit.Scale.X;
                        var pctY = movement.DeltaLocationInView.Y / movement.Hit.WidgetRect.Height / movement.Hit.Scale.Y;

                        if (device.FlowDirection == FlowDirection.RightToLeft)
                            pctX = -pctX;

                        move = new SKPoint(pctX, pctY);

                    }
                    else if (device.FlowDirection == FlowDirection.RightToLeft)
                        move.X = -move.X;


                    //System.Diagnostics.Debug.WriteLine($"pan: {move}");

                    Task.Run(() => OnPanMove(move));
                }

                return (false, false);
            }

            public override void Released(TimeSpan duration)
            {
                base.Released(duration);
                if (OnPanEnd != null)
                {
                    var velocity = Velocity;
                    Task.Run(() => OnPanEnd(velocity));
                }
                LastVelocityMovement.StartTime = default;
                Velocity = new SKPoint();
            }
        }


    }

}
