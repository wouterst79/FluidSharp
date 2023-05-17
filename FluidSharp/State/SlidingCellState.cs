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
    public class SlidingCellState
    {

        private object Context;

        public float StartingPan;
        public float AdditionalPan; // absolute
        public float Open;

        public DateTime? OpenStarted;
        public DateTime? CloseStarted;

        public SlidingCellState() { }

        public const float FlingingVelocity = 200; // pixels per seconds
        public const float OpenThreshold = 20; // pixels


        public bool IsContext(object context)
        {
            if (Context == null) return context == null;
            return Context.Equals(context);
        }

        public void CollapseOnDifferentTouchTarget(TouchTarget touchTarget)
        {
            if (Context != null && !CloseStarted.HasValue)
            {
                if (!touchTarget.IsContext(Context)) 
                {
                    // touching another widget, while thisone is open. close it now
                    CloseStarted = DateTime.UtcNow;
                }
            }
        }

        /// <summary>
        /// Calculate the % of opening on the cell, with 1 representing 100% open on the near side, and -1 100% on the far side
        /// </summary>
        public (float ratio, bool isanimating) GetOpenRatio(float width)
        {

            if (width == 0)
            {
                Open = 0;
                return (0, false);
            }

            var isanimating = false;

            var pan = StartingPan + AdditionalPan;
            if (pan > width) pan = width;
            if (pan < -width) pan = -width;

            if (CloseStarted.HasValue)
            {
                var delta = (float)(DateTime.UtcNow.Subtract(CloseStarted.Value).TotalMilliseconds / SlidingCell.DefaultDuration.TotalMilliseconds);
                if (delta > 1) delta = 1; else isanimating = true;
                delta = 1 - Easing.CubicOut.Ease(delta);
                Open = pan * delta;
            }
            else if (OpenStarted.HasValue)
            {
                var delta = (float)(DateTime.UtcNow.Subtract(OpenStarted.Value).TotalMilliseconds / SlidingCell.DefaultDuration.TotalMilliseconds);
                if (delta > 1) delta = 1; else isanimating = true;
                delta = Easing.CubicOut.Ease(delta);
                if (pan > 0)
                    Open = pan + (width - pan) * delta;
                else
                    Open = pan - (width + pan) * delta;
            }
            else
            {
                Open = pan;
            }

            //System.Diagnostics.Debug.WriteLine($"cell state open {Open} {width} {Open / width} {OpenStarted.HasValue} {CloseStarted.HasValue} {isanimating}");

            return (Open / width, isanimating);
        }

        public async Task SetPan(object context, float pan, VisualState visualState)
        {

            //System.Diagnostics.Debug.WriteLine($"setting pan: {pan} {StartingPan}");

            if (Context == null)
            {
                Context = context;
                StartingPan = 0;
                AdditionalPan = pan;
                OpenStarted = null;
                CloseStarted = null;
            }
            else
            if (IsContext(context))
            {

                if (OpenStarted.HasValue || CloseStarted.HasValue)
                {
                    StartingPan = Open;
                    //System.Diagnostics.Debug.WriteLine($"setting starting pan: {StartingPan}");
                }

                AdditionalPan = pan;
                OpenStarted = null;
                CloseStarted = null;
            }
            else if (OpenStarted.HasValue && !CloseStarted.HasValue)
            {
                // start closing the other on
                CloseStarted = DateTime.UtcNow;
            }
            else if (CloseStarted.HasValue)
            {
                if (!GetOpenRatio(100).isanimating)
                {
                    // switch context
                    Context = context;
                    StartingPan = 0;
                    AdditionalPan = pan;
                    OpenStarted = null;
                    CloseStarted = null;
                }
            }

            await visualState.RequestRedraw();

        }

        public async Task EndPan(SKPoint velocity, VisualState visualState)
        {

            var flinging = Math.Abs(velocity.X) > FlingingVelocity;
            var pan = StartingPan + AdditionalPan;

            if (flinging)
            {
                if (pan > 0)
                {
                    if (velocity.X > 0) OpenStarted = DateTime.UtcNow; else CloseStarted = DateTime.UtcNow;
                }
                else
                {
                    if (velocity.X < 0) OpenStarted = DateTime.UtcNow; else CloseStarted = DateTime.UtcNow;
                }
            }
            else
            {
                if (pan > 0)
                {
                    if (pan > OpenThreshold) OpenStarted = DateTime.UtcNow; else CloseStarted = DateTime.UtcNow;
                }
                else
                {
                    if (pan < -OpenThreshold) OpenStarted = DateTime.UtcNow; else CloseStarted = DateTime.UtcNow;
                }
            }

            await visualState.RequestRedraw();

        }



    }
}
