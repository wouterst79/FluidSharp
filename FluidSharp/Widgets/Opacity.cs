using FluidSharp.Layouts;
using FluidSharp.Paint;
using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Text;

namespace FluidSharp.Widgets
{
    public class Opacity : Widget
    {

        public float Factor { get; set; }
        public Widget? Contents { get; set; }

        public Opacity(float factor, Widget? contents)
        {
            Factor = factor;
            Contents = contents;
        }

        public override SKSize Measure(MeasureCache measureCache, SKSize boundaries) => Contents == null ? boundaries : Contents.Measure(measureCache, boundaries);
        public override SKRect PaintInternal(LayoutSurface layoutsurface, SKRect rect)
        {
            if (Contents == null) return rect;

            if (layoutsurface.Canvas == null)
                return layoutsurface.Paint(Contents, rect);

#if DEBUG
            //layoutsurface.SetHasActiveAnimations();
#endif

            if (Factor == 0)
            {
                var height = Measure(layoutsurface.MeasureCache, rect.Size).Height;
                return rect.WithHeight(height);
            }

            if (Factor < 1)
            {

                using (var recorder = new SKPictureRecorder())
                {

                    var originalcanvas = layoutsurface.Canvas;
                    var recording = recorder.BeginRecording(rect);
                    layoutsurface.SetCanvas(recording);

                    var result = layoutsurface.Paint(Contents, rect);

                    layoutsurface.SetCanvas(originalcanvas);
                    using (var recorded = recorder.EndRecording()) //.Snapshot();
                    {
                        originalcanvas.DrawPicture(recorded, PaintCache.GetOpacityPaint(Factor));
                    }

#if DEBUG
                    layoutsurface.DebugNewRect(rect, SKColors.Yellow.WithAlpha(32));
#endif

                    return result;

                }


            }
            else
            {
                var result = layoutsurface.Paint(Contents, rect);
                return result;
            }

        }

    }
}
