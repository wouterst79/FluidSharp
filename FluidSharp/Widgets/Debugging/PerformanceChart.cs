using FluidSharp.Engine;
using FluidSharp.Layouts;
using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Text;
using SKPaint1 = SkiaSharp.SKPaint;

namespace FluidSharp.Widgets.Debugging
{
    public class PerformanceChart : Widget
    {

        SKPaint1 paint1 = new SKPaint1() { Color = SKColors.Green };
        SKPaint1 paint2 = new SKPaint1() { Color = SKColors.Blue };
        SKPaint1 paint3 = new SKPaint1() { Color = SKColors.Red };
        //SKPaint1 scrollpaint = new SKPaint1() { Color = SKColors.Purple };

        SKPaint1 textpaint = new SKPaint1() { Color = SKColors.White, TextSize = 14 };
        SKPaint1 goalpaint = new SKPaint1() { Color = SKColors.Gray };


        private PerformanceChart() { }

        private static PerformanceChart? instance;
        public static PerformanceChart Instance => instance ?? (instance = new PerformanceChart());

        public override SKSize Measure(MeasureCache measureCache, SKSize boundaries)
        {
            return boundaries;
        }

        public override SKRect PaintInternal(LayoutSurface layoutsurface, SKRect rect)
        {

            var canvas = layoutsurface.Canvas;
            if (canvas == null) return rect;

            var tracker = layoutsurface.VisualState.PerformanceTracker;

            if (tracker is NoopPerformanceTracker) layoutsurface.VisualState.PerformanceTracker = new SimplePerformanceTracker();

            if (tracker == null)
            {
                // draw text
                var text = $"no performance information collected";
                canvas.DrawText(text, 0, 20, textpaint);
            }

            var frames = tracker.GetFrames();
            if (tracker.GetTime() > 2000)
                tracker.ForceRestart();

            if (frames.Count == 0)
                return rect;

            var t0 = frames[0].paintstart;

            {

                var xfactor = 1f / 8f;
                var yfactor = 10f;
                //var animyfactor = .3f;

                //TimeSpan? scroll0 = null;

                for (int i = 0; i < frames.Count; i++)
                {

                    var frame = frames[i];

                    var x = (float)(1 + (frame.paintstart - t0) * xfactor);

                    var t1 = (float)((frame.paintstart - frame.requested) * yfactor);
                    var t2 = (float)((frame.widgetscreated - frame.paintstart) * yfactor);
                    var t3 = (float)((frame.paintfinished - frame.widgetscreated) * yfactor);

                    if (frame.requested == 0)
                        t1 = 1;

                    var y = 0f;
                    canvas.DrawRect(new SKRect(x, y, x + 1, y += t1), paint1);
                    canvas.DrawRect(new SKRect(x, y, x + 1, y += t2), paint2);
                    canvas.DrawRect(new SKRect(x, y, x + 1, y += t3), paint3);

                    x++;

                }

                // goal line
                {
                    var y = 16 * yfactor;
                    canvas.DrawLine(0, y, 1000, y, goalpaint);
                }

            }


            return rect;
        }
    }
}
