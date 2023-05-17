using FFMediaToolkit;
using FFMediaToolkit.Encoding;
using FFMediaToolkit.Graphics;
using FluidSharp.Layouts;
using FluidSharp.State;
using FluidSharp.Widgets;
using SkiaSharp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;

namespace FluidSharp.Video.Recorder
{

    public class VideoRecorderLayoutSurface : LayoutSurface
    {

        public CancellationTokenSource CancellationTokenSource;



        public VideoRecorderLayoutSurface(Device device, MeasureCache measureCache, string FFmpegPath) : base(device, measureCache, null, new VisualState(null, null))
        {
            if (FFmpegLoader.FFmpegPath != FFmpegPath)
                FFmpegLoader.FFmpegPath = FFmpegPath;// @"C:\util\ffmpeg-4.2.1-win-64\bin";
        }

        public void Stop()
        {
            CancellationTokenSource.Cancel();
        }

        public async Task Run(string fullfilename, SKSize size, float scale, int fps, Func<Widget> makeWidget)
        {


            var MSBetweenFrame = 1000f / fps;

            var w = (((int)(size.Width * scale)) / 4) * 4;
            var h = (((int)(size.Height * scale)) / 4) * 4;

            var framesize = new System.Drawing.Size(w, h);

            var settings = new VideoEncoderSettings(width: w, height: h, framerate: fps, codec: VideoCodec.H264);
            settings.EncoderPreset = EncoderPreset.Fast;
            settings.CRF = 17;

            CancellationTokenSource = new CancellationTokenSource();

            var framelen = TimeSpan.FromMilliseconds(MSBetweenFrame);
            var tframe = DateTime.UtcNow;

            var frameid = 0;
            using (var outfile = MediaBuilder.CreateContainer(fullfilename).WithVideo(settings).Create())
            {
                while (!CancellationTokenSource.IsCancellationRequested)
                {

                    DrawFrame(outfile, makeWidget, framesize, scale);

                    tframe = tframe.Add(framelen);
                    var twait = tframe.Subtract(DateTime.UtcNow);
                    if (twait.TotalMilliseconds > 0)
                    {
                        System.Diagnostics.Debug.WriteLine($"waiting {twait.TotalMilliseconds} ms");
                        await Task.Delay(twait);
                    }
                    else
                    {
                        System.Diagnostics.Debug.WriteLine($"WARNING: producing frame took more than {MSBetweenFrame} ms");
                    }

                    frameid++;
                }
            }
        }


        private unsafe void DrawFrame(MediaOutput outfile, Func<Widget> makeWidget, System.Drawing.Size framesize, float scale)
        {

            var widget = makeWidget();
            widget = new Scale(scale, widget);


            using (var bitmap = new SKBitmap(new SKImageInfo((int)framesize.Width, (int)framesize.Height, SKImageInfo.PlatformColorType, SKAlphaType.Premul)))
            using (var canvas = new SKCanvas(bitmap))
            {

                //surface.Canvas.Clear(SKColors.Transparent);
                canvas.Clear(SKColors.White);

                SetCanvas(canvas);

                Paint(widget, new SKRect(0, 0, framesize.Width, framesize.Height));


                // add frame
                var bpp = 4;
                var buffersize = bpp * framesize.Width * framesize.Height;

                var span = new Span<byte>(bitmap.GetPixels().ToPointer(), buffersize);
                var img = new ImageData(span, ImagePixelFormat.Bgra32, framesize);

                outfile.Video.AddFrame(img);



            }

        }


    }
}