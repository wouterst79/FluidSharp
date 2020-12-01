using FluidSharp.Widgets.Native;
using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Text;

namespace FluidSharp.Interop
{
    public interface INativeViewManager
    {

        SKSize Measure(NativeViewWidget nativeViewWidget, SKSize boundaries);

        void PaintStarted();
        void UpdateNativeView(NativeViewWidget nativeViewWidget, SKRect rect, SKRect original);
        void PaintCompleted();

    }
}
