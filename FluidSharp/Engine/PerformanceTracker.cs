using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading.Tasks;

namespace FluidSharp.Engine
{

    public interface IPerformanceTracker
    {
        void AnimationFinished();
        void ForceRestart();
        List<FrameInfo> GetFrames();
        double GetTime();
        void PaintFinished();
        void PaintStart();
        void Request();
        void WidgetsCreated();
    }

    public struct FrameInfo
    {
        public double requested;
        public double paintstart;
        public double widgetscreated;
        public double paintfinished;
    }

    public class NoopPerformanceTracker:IPerformanceTracker
    {
        public void AnimationFinished() { }
        public void ForceRestart() { }
        public List<FrameInfo> GetFrames() => new List<FrameInfo>();
        public double GetTime() => 0;
        public void PaintFinished() { }
        public void PaintStart() { }
        public void Request() { }
        public void WidgetsCreated() { }
    }

    public class SimplePerformanceTracker : IPerformanceTracker
    {

        private FrameInfo CurrentFrame;
        private List<FrameInfo> Frames = new List<FrameInfo>();
        private bool Restart;

        private Stopwatch stopwatch;

        private const int MaxFramesInHistory = 1000;

        private const int MaxMSPerFrameWarning = 1000 / 60;
        private const int MaxMSPerFrameError = 1000 / 10;

        public Func<FrameInfo, string, Task> OnPerformanceProblem = async (frame, message) => Debug.WriteLine($"PERFORMANCE: {message}");

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

        public void ForceRestart()
        {
            Debug.WriteLine("restarting");
            stopwatch = null; 
            Frames.Clear();
            Restart = false;
            CurrentFrame = new FrameInfo();
            Frames.Add(CurrentFrame);
        }

        public List<FrameInfo> GetFrames() => Frames;

    }
}
