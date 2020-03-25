using FluidSharp.Animations;
using FluidSharp.Touch;
using FluidSharp.Widgets;
using FluidSharp.Widgets.CrossPlatform;
using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace FluidSharp.State
{

    public class SlideOverState
    {

        // Context
        private object Context;

        public TimeSpan Duration = SlideOverContainer.DefaultDuration;

        // Milestones
        public DateTime? OpenStarted;
        public DateTime? CloseStarted;


        public const float FlingingVelocity = 30; // pixels per seconds


        // State accessors
        public bool IsContext(object context) => TypedContext.ContextEqual(Context, context);

        public bool IsClosing => CloseStarted.HasValue && CloseStarted.Value.Add(Duration) > DateTime.Now;
        public bool IsClosed => !OpenStarted.HasValue && !IsClosing;

        public bool IsOpening => !CloseStarted.HasValue && OpenStarted.HasValue && OpenStarted.Value.Add(Duration) > DateTime.Now;
        public bool IsOpen => OpenStarted.HasValue && !CloseStarted.HasValue && !IsOpening;


        private SlideOverState() { }



        // Static access
        public static (float ratio, bool isanimating) GetRatio(VisualState visualState, object context)
        {
            var state = GetState(visualState);
            if (state != null && state.IsContext(context))
            {
                var ratio = state.GetOpenRatio();
                //System.Diagnostics.Debug.WriteLine($"open ratio: {ratio}");
                return ratio;
            }
            return (0, false);
        }


        public static SlideOverState GetState(VisualState visualState) => visualState.GetOrMake("SlideOverState", () => new SlideOverState());


        public static Task Open(VisualState visualState, object context)
        {

            var state = GetState(visualState);

            if (state.Context != null && !state.IsContext(context))
            {
                if (state.IsOpen)
                {
                    // start closing other context
                    state.CloseStarted = DateTime.Now;
                    state.OpenStarted = null;
                    return visualState.RequestRedraw();
                }
                else if (state.IsOpening)
                {
                    // start closing other context, start at current ratio
                    return Close(visualState, state.Context);
                }
                else if (state.IsClosing)
                {
                    // finish closing other context
                    return visualState.RequestRedraw();
                }
            }

            state.Context = context;

            if (state.IsOpen || state.IsOpening) 
                return Task.CompletedTask;

            if (state.IsClosing)
            {
                // open it starting current ratio
                var closed = 1 - state.GetOpenRatio().ratio; //  ie "90% closed"
                var completed = Easing.CubicOutInverse.Ease(closed); // ie how much time would be passed to get 90% closed (ie 2/3 of duration)
                var ms = state.Duration.TotalMilliseconds;
                state.OpenStarted = DateTime.Now.AddMilliseconds((completed - 1) * ms);
                state.CloseStarted = null;
            }
            else
            {
                // start opening
                state.OpenStarted = DateTime.Now;
                state.CloseStarted = null;
            }

            return visualState.RequestRedraw();

        }


        public static Task Close(VisualState visualState, object context)
        {
            var state = GetState(visualState);

            if (!state.IsContext(context))
                return Task.CompletedTask;

            if (state.IsClosed || state.IsClosing) 
                return Task.CompletedTask;

            if (state.IsOpening)
            {
                // close it starting current ratio
                var open = state.GetOpenRatio().ratio; // ie 90% open
                var completed = Easing.CubicIn.Ease(open); //  ie how much time would be passed to get 90% closed (ie 2/3 of duration)
                var ms = state.Duration.TotalMilliseconds;
                state.OpenStarted = null;
                state.CloseStarted = DateTime.Now.AddMilliseconds((completed - 1) * ms);
            }
            else
            {
                // start closing
                state.OpenStarted = null;
                state.CloseStarted = DateTime.Now;
            }

            return visualState.RequestRedraw();

        }



        public (float ratio, bool isanimating) GetOpenRatio()
        {

            var isanimating = false;

            if (CloseStarted.HasValue)
            {
                var delta = (float)(DateTime.Now.Subtract(CloseStarted.Value).TotalMilliseconds / Duration.TotalMilliseconds);
                if (delta > 1) delta = 1; else isanimating = true;
                return (1 - Easing.CubicOut.Ease(delta), isanimating);
            }
            else if (OpenStarted.HasValue)
            {
                var delta = (float)(DateTime.Now.Subtract(OpenStarted.Value).TotalMilliseconds / Duration.TotalMilliseconds);
                if (delta > 1) delta = 1; else isanimating = true;
                return (Easing.CubicOut.Ease(delta), isanimating);
            }
            else
            {
                System.Diagnostics.Debug.Assert(!IsOpen && IsClosed);
                return (0, false);
            }
        }


        public Task EndPan(SKPoint velocity, VisualState visualState)
        {

            var flinging = Math.Abs(velocity.X) > FlingingVelocity;
            if (flinging)
                return Close(visualState, Context);

            return Task.CompletedTask;

        }



    }
}
