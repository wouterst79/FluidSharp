using FluidSharp.Interop;
using FluidSharp.Paint.Images;
using SkiaSharp.TextBlocks;
using System;
using System.Collections.Generic;
using System.Text;

namespace FluidSharp.Layouts
{

    /// <summary>
    /// The Measure Context represents View level layout parameters
    /// </summary>
    public class MeasureCache : IDisposable
    {

        public TextShaper TextShaper;
        public ImageCache ImageCache;

        public INativeViewManager NativeViewManager;

        public MeasureCache(Device device, Action onDataAvailable, INativeViewManager nativeViewManager)
        {
            TextShaper = new TextShaper(true, device.FontSizeScale);
            ImageCache = new ImageCache(onDataAvailable);
            NativeViewManager = nativeViewManager;
        }

        public void Dispose()
        {
            TextShaper.Dispose();
        }

    }
}
