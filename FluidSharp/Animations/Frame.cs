using System;

namespace FluidSharp.Animations
{
    public class Frame : Animation
    {

        // Frame
        public string Name { get; set; }
        public TimeSpan FrameStart { get; set; }
        public TimeSpan FrameEnd => FrameStart + Duration;

        public TimeSpan GetDurationPct(float pct) => FrameStart + Duration * pct;

        public Frame(string name, float durationMS, Easing? easing = null)
            : this(Speed, name, TimeSpan.Zero, TimeSpan.FromMilliseconds(durationMS), easing)
        { }

        public Frame(string name, float startMS, float durationMS, Easing? easing = null)
            : this(Speed, name, TimeSpan.FromMilliseconds(startMS), TimeSpan.FromMilliseconds(durationMS), easing)
        { }

        public Frame(string name, TimeSpan start, TimeSpan duration, Easing? easing = null)
            : this(Speed, name, start, duration, easing)
        {
        }

        private Frame(float speed, string name, TimeSpan start, TimeSpan duration, Easing? easing = null)
        {
            Name = name ?? throw new ArgumentNullException(nameof(name));
            FrameStart = start * speed;
            Duration = duration * speed;
            Easing = easing;
        }

        public Frame Reverse()
        {
            StartValue = 1; EndValue = 0;
            return this;
        }

        public Frame SetStartEnd(float start, float end)
        {
            StartValue = start;
            EndValue = end;
            return this;
        }

    }
}
