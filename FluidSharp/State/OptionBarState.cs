using FluidSharp.Animations;
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
    public class OptionBarState
    {

        public int AtScrollStart;
        public int Current;
        public DateTime? ScrollStart;

        public TimeSpan ScrollDuration => TimeSpan.FromMilliseconds(350);

        public OptionBarState(int value)
        {
            Current = value;
        }

        public void SetCurrent(int value)
        {
            if (Current == value) return;
            AtScrollStart = Current;
            Current = value;
            ScrollStart = DateTime.UtcNow;
        }

        public (float scroll, bool isanimating) GetScroll()
        {

            if (ScrollStart.HasValue)
            {
                var d = (float)(DateTime.UtcNow.Subtract(ScrollStart.Value).TotalMilliseconds / ScrollDuration.TotalMilliseconds);

                if (d > 1) 
                    return (Current, false);

                d = Easing.CubicOut.Ease(d);
                var scroll = AtScrollStart + (Current - AtScrollStart) * d;

                return (scroll, true);
            }
            else
            {
                return (Current, false);
            }

        }

    }
}
