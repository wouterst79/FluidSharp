using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Text;

namespace FluidSharp.Widgets.Native
{
    public interface INativeViewImpl
    {

        void UpdateControl(NativeViewWidget nativeViewWidget, SKRect rect, SKRect original);

    }
}
