using FluidSharp.Interop;
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
        public INativeViewManager NativeViewManager;

        public MeasureCache(Device device, INativeViewManager nativeViewManager)
        {
            TextShaper = new TextShaper(true, device.FontSizeScale);
            NativeViewManager = nativeViewManager;
        }

        public void Dispose()
        {
            TextShaper.Dispose();
        }

    }
}
