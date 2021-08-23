using FluidSharp.Layouts;
using FluidSharp.Samples.All;
using FluidSharp.State;
using SkiaSharp;
using System;
using System.Diagnostics;
using System.IO;

namespace FluidSharp.Samples.Screenshots
{
    class Program
    {

        private const int TargetWidth = 400;

        static void Main(string[] args)
        {

            // make output folder
            var outputfolder = Path.Combine(Environment.CurrentDirectory, @"..\..\..\..\Screenshots");
            outputfolder = new DirectoryInfo(outputfolder).FullName;
            Directory.CreateDirectory(outputfolder);

            var sampleApp = new SampleApp();

            var device = new Device();
            var measureCache = new MeasureCache(device, null, null);
            var visualState = new VisualState(null, null);

            foreach (var sample in sampleApp.Samples)
            {

                var filename = $"{sample.Name}.png";
                var FullFilename = Path.Combine(outputfolder, filename);

                // make sample widget
                var widget = sample.MakeWidget(visualState);
                var size = widget.Measure(measureCache, new SKSize(TargetWidth, 0));

                if (size.Width == 0) size = new SKSize(400, 400);

                // create a surface
                using (var Surface = SKSurface.Create(new SKImageInfo((int)size.Width, (int)size.Height, SKImageInfo.PlatformColorType, SKAlphaType.Premul)))
                {

                    var canvas = Surface.Canvas;

                    // start with white
                    canvas.Clear(SKColors.White);

                    var layoutsurface = new LayoutSurface(device, measureCache, canvas, visualState);
                    var actual = layoutsurface.Paint(widget, new SKRect(0, 0, size.Width, size.Height));

                    // save the sample
                    using (var outstream = new FileStream(FullFilename, FileMode.Create))
                    using (var pixmap = Surface.Snapshot().PeekPixels())
                    using (var data = pixmap.Encode(new SKPngEncoderOptions()))
                        data.SaveTo(outstream);

                }

            }

            // open the output folder
            Process.Start(new ProcessStartInfo() { FileName = outputfolder, UseShellExecute = true });

        }
    }
}
