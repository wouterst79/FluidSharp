using FluidSharp.Layouts;
using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace FluidSharp.Widgets.Debugging
{
    public class FrameCounter : Widget
    {

        private static int framecount = 0;
        private static LinkedList<DateTime> tracker = new LinkedList<DateTime>();
        private static LinkedList<long> tracker2 = new LinkedList<long>();

        private static Stopwatch stopwatch;

        private const int listlength = 25;

        public override SKSize Measure(MeasureCache measureCache, SKSize boundaries)
        {
            return boundaries;
        }

        public override SKRect PaintInternal(LayoutSurface layoutsurface, SKRect rect)
        {

            var canvas = layoutsurface.Canvas;
            if (canvas != null)
            {

                if (stopwatch == null)
                {
                    stopwatch = new Stopwatch();
                    stopwatch.Start();
                }

                var now = DateTime.Now;
                tracker.AddLast(now);
                tracker2.AddLast(stopwatch.ElapsedMilliseconds);

                while (tracker.Count > listlength)
                    tracker.RemoveFirst();

                while (tracker2.Count > listlength)
                    tracker2.RemoveFirst();

                var first = tracker.First;
                var duration = now.Subtract(first.Value).TotalMilliseconds;
                var count = tracker.Count;

                var fps = Math.Round(count * 1000 / duration);

                using (var framepaint = new SKPaint() { Color = SKColors.Red })
                {
                    canvas.DrawRect(0, 0, (++framecount % 60), 10, framepaint);
                    canvas.DrawRect(0, 10, (float)fps, 10, framepaint);
                    canvas.DrawRect(0, 20, 60, 5, framepaint);

                    var x = 0f;
                    var node = tracker.First;
                    var firsttime = node.Value;
                    while (node != null)
                    {
                        var delta = (float)node.Value.Subtract(firsttime).TotalMilliseconds / 3;
                        x = delta;
                        canvas.DrawLine(x, 25, x, 30, framepaint);
                        node = node.Next;
                    }

                    var node2 = tracker2.First;
                    var firsttime2 = node2.Value;
                    while (node2 != null)
                    {
                        var delta = (node2.Value - firsttime2) / 3f;
                        x = delta;
                        canvas.DrawLine(x, 30, x, 35, framepaint);
                        node2 = node2.Next;
                    }

                }

                using (var textpaint = new SKPaint() { Color = SKColors.White, TextSize = 14 })
                {
                    var text = $"{fps} / {framecount}";
                    canvas.DrawText(text, 0, 20, textpaint);
                }

            }

            return new SKRect(rect.Left, rect.Top, rect.Right, rect.Top);
        }
    }
}
