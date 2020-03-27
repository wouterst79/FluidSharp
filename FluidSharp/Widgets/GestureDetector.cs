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

        public object Context;

        public bool IsMultiTouch;
        public Func<SKPoint, Task> OnTouchDown;
        public Func<Task> OnWin;
        public Func<Task> OnTouchUp;

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

            public DateTime StartTime;
            public DateTime CurrentTime;
            public TimeSpan DeltaTime;

            public bool HasWon;

            public bool IsInitialPointer;

            public Movement(HitTestHit hit, SKPoint startLocationOnDevice, SKPoint currentLocationOnDevice, SKPoint startLocationInView, SKPoint currentLocationInView, DateTime startTime, DateTime currentTime, bool hasWon, bool isInitialPointer) : this()
            {

                Hit = hit;

                StartLocationOnDevice = startLocationOnDevice;
                CurrentLocationOnDevice = currentLocationOnDevice;
                DeltaLocationOnDevice = currentLocationOnDevice - startLocationOnDevice;

                StartLocationInView = startLocationInView;
                CurrentLocationInView = currentLocationInView;
                DeltaLocationInView = currentLocationInView - startLocationInView;

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

        public static TapGestureDetector TapDetector(VisualState visualState, object context, Func<Task> OnTapped, Func<Task> OnLongTapped, Widget child)
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
            Invoke(OnTouchUp);
        }

        public virtual void Cancelled()
        {
            Invoke(OnTouchUp);
        }

        public void Invoke(Func<Task> func)
        {
            if (func != null) Task.Run(func);
        }


        // Widget Implementation
        public override SKSize Measure(MeasureCache measureCache, SKSize boundaries) => Child.Measure(measureCache, boundaries);
        public override SKRect PaintInternal(LayoutSurface layoutsurface, SKRect rect) => layoutsurface.Paint(Child, rect);

        // Detectors
        public class TapGestureDetector : GestureDetector
        {

            public Func<Task> OnTapped;
            public Func<Task> OnLongTapped;

            public static TimeSpan LongTapDuration = TimeSpan.FromMilliseconds(500);

            public TapGestureDetector(VisualState visualState, object context, Func<Task> onTapped, Func<Task> onLongTapped, Widget child) : base(visualState, context, child)
            {
                OnTouchDown = async (location) => visualState.TouchTarget = new TouchTarget(new TapContext(context), location);
                OnTouchUp = async () => visualState.TouchTarget = new TouchTarget();
                OnTapped = onTapped;
                OnLongTapped = onLongTapped;
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

            public override void Released(TimeSpan duration)
            {
                base.Released(duration);
                if (duration > LongTapDuration && OnLongTapped != null)
                    Invoke(OnLongTapped);
                else
                    Invoke(OnTapped);
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
                    Task.Run(() => OnPanEnd(Velocity));
            }
        }


    }

}
