using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading.Tasks;

namespace FluidSharp.Engine
{

    public class PerformanceTracker
    {

        public struct FrameInfo
        {
            public double requested;
            public double paintstart;
            public double widgetscreated;
            public double paintfinished;
        }

        private FrameInfo CurrentFrame;
        private List<FrameInfo> Frames = new List<FrameInfo>();
        private bool Restart;

        private Stopwatch stopwatch;

        private const int MaxFramesInHistory = 1000;

        private const int MaxMSPerFrameWarning = 1000 / 60;
        private const int MaxMSPerFrameError = 1000 / 10;

        public Func<FrameInfo, string, Task> OnPerformanceProblem = async (frame, message) => System.Diagnostics.Debug.WriteLine($"PERFORMANCE: {message}");

        // timing
        public double GetTime()
        {
            if (stopwatch == null) { stopwatch = new Stopwatch(); stopwatch.Start(); }
            return stopwatch.Elapsed.TotalMilliseconds;
        }


        // milestones
        public void Request()
        {
            if (Restart) { Frames.Clear(); Restart = false; }
            CurrentFrame = new FrameInfo() { paintstart = GetTime() };
        }
        public void PaintStart() => CurrentFrame.paintstart = GetTime();
        public void WidgetsCreated() => CurrentFrame.widgetscreated = GetTime();
        public void PaintFinished()
        {
            CurrentFrame.paintfinished = GetTime();

            if (Frames.Count >= MaxFramesInHistory)
                Frames.Clear();

            Frames.Add(CurrentFrame);
        }

        public void AnimationFinished()
        {
            Restart = true;
        }

        public List<FrameInfo> GetFrames() => Frames;

    }
}
