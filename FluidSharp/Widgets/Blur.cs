using FluidSharp.Layouts;
using FluidSharp.Paint;
using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Text;

namespace FluidSharp.Widgets
{
    public class Blur : Widget
    {

        public float Sigma { get; set; }
        public Widget Contents { get; set; }

        public Blur(float sigma, Widget contents)
        {
            Sigma = sigma;
            Contents = contents ?? throw new ArgumentNullException(nameof(contents));
        }

        public override SKSize Measure(MeasureCache measureCache, SKSize boundaries) => Contents.Measure(measureCache, boundaries);
        public override SKRect PaintInternal(LayoutSurface layoutsurface, SKRect rect)
        {

            if (layoutsurface.Canvas == null || Sigma == 0)
                return layoutsurface.Paint(Contents, rect);

            using (var recorder = new SKPictureRecorder())
            {

                var originalcanvas = layoutsurface.Canvas;
                var recording = recorder.BeginRecording(rect);
                layoutsurface.SetCanvas(recording);

                var result = layoutsurface.Paint(Contents, rect);

                var recorded = recorder.EndRecordingAsDrawable().Snapshot();

                layoutsurface.SetCanvas(originalcanvas);

                using (var filter = SKImageFilter.CreateBlur(Sigma, Sigma))
                    originalcanvas.DrawPicture(recorded, PaintCache.GetBackgroundPaint(SKColors.Red, true, () => filter));

                return result;

            }

        }

    }
}
