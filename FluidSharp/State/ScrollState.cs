﻿using FluidSharp.Animations;
using FluidSharp.Engine;
using FluidSharp.Widgets;
using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using static FluidSharp.Widgets.Scrollable;

namespace FluidSharp.State
{
    public class ScrollState
    {

        public OverscrollBehavior OverscrollBehavior;

        private float Scroll;

        public float? Pan;
        public DateTime? LastPanEnd;
        public DateTime? BoundaryHit;
        public float EndVelocity; // Pixels per second

        public TimeSpan OverscrollDuration => TimeSpan.FromMilliseconds(350);

        public const float FlingingVelocity = 200; // pixels per seconds

        public float Minimum;

        //private const float 

        public ScrollState(OverscrollBehavior overscrollBehavior)
        {
            OverscrollBehavior = overscrollBehavior;
        }

        public async Task SetPan(float pan, VisualState visualState)
        {

            if (!Pan.HasValue)
            {
                var current = GetScroll();
                //System.Diagnostics.Debug.WriteLine($"SetPan restart: {pan} ({current})");
                Scroll = current.scroll;
            }

            Pan = pan;
            LastPanEnd = null;
            BoundaryHit = null;

            //System.Diagnostics.Debug.WriteLine($"setting pan: {pan} ({Scroll}) = {Scroll + Pan}");

            await visualState.RequestRedraw();

        }

        public async Task EndPan(SKPoint velocity, VisualState visualState)
        {

            if (Pan.HasValue)
            {
                Scroll = Scroll + Pan.Value;
                Pan = null;
                LastPanEnd = DateTime.Now;
                BoundaryHit = null;
                EndVelocity = velocity.Y;
            }

            //System.Diagnostics.Debug.WriteLine($"pan ended: {Scroll} ({Scroll + Pan}) - velocity: {EndVelocity}");

            await visualState.RequestRedraw();

        }

        public void SetRange(float height, float contentsheight)
        {

            var min = height - contentsheight;
            if (min > 0) min = 0;
            Minimum = min;

            //System.Diagnostics.Debug.WriteLine($"minimum set: {Minimum} ({contentsheight} / {height})");

        }

        public (float scroll, float overscroll, bool hasactiveanimations) GetScroll()
        {

            var pan = Pan;
            var lastPanEnd = LastPanEnd;
            var boundaryHit = BoundaryHit;


            var scroll = Scroll;
            if (pan.HasValue) scroll += pan.Value;

            // calculate partial pan for overscroll panning
            if (scroll < Minimum)
            {
                if (Minimum < 0) // only if the contents is longer than the list
                    scroll = Minimum + (scroll - Minimum) / 2;
                else
                    scroll = Minimum;
            }
            if (scroll > 0)
            {
                if (Minimum < 0)
                    scroll = scroll / 2;
                else
                    scroll = 0;
            }

            // add fling extra
            var hasactiveanimations = false;
            if (lastPanEnd.HasValue && Math.Abs(EndVelocity) > FlingingVelocity)
            {
                var timespan = DateTime.Now.Subtract(lastPanEnd.Value);
                var seconds = timespan.TotalMilliseconds / 1000;
                var factor = 1 - Math.Exp(-1.5 * seconds);
                var extra = (float)(-EndVelocity / 2 * factor);
                //System.Diagnostics.Debug.WriteLine($"time factor: {factor}, extra: {extra}, fling: {FlingingVelocity}");
                scroll -= extra;
                hasactiveanimations = factor < .98;
            }

            // separate out scroll, and overscroll
            var overscroll = 0f;
            if (scroll < Minimum)
            {
                if (Minimum < 0)
                    overscroll = scroll - Minimum;
                scroll = Minimum;
            }
            if (scroll > 0)
            {
                if (Minimum < 0)
                    overscroll = scroll;
                scroll = 0;
            }

            // start overscroll bounce
            if (overscroll != 0 && lastPanEnd.HasValue && !boundaryHit.HasValue)
                BoundaryHit = boundaryHit = DateTime.Now;

            //System.Diagnostics.Debug.WriteLine($"Scroll: {Scroll} sc: {scroll} os:{overscroll}");

            if (boundaryHit.HasValue)
            {

                var dt = 1 - (DateTime.Now.Subtract(boundaryHit.Value).TotalMilliseconds / OverscrollDuration.TotalMilliseconds);
                dt = Easing.CubicIn.Ease(dt);
                if (dt < 0) dt = 0;

                overscroll = (float)(overscroll * dt);
                if (dt > 0)
                    hasactiveanimations = true;

            }

            // add overscroll back into scroll for "stretch" behavior
            if (OverscrollBehavior == OverscrollBehavior.Stretch)
                scroll += overscroll;

            if (!pan.HasValue && !hasactiveanimations)
            {

                //System.Diagnostics.Debug.WriteLine("ending animations");

                // deal with widget resizing
                if (scroll < Minimum)
                    scroll = Minimum;

                Scroll = scroll;
                LastPanEnd = null;
                EndVelocity = 0;
            }

            //System.Diagnostics.Debug.WriteLine($"Scroll: {Scroll} sc: {scroll} os:{overscroll}");

            return (scroll, overscroll, hasactiveanimations);

        }


    }
}
