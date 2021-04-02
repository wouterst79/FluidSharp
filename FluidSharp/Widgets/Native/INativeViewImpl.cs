using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Text;

namespace FluidSharp.Widgets.Native
{
    public interface INativeViewImpl
    {

        void SetVisible(bool visible);
        void SetBounds(SKRect nativebounds);

        void UpdateControl(NativeViewWidget nativeViewWidget, SKRect rect, SKRect original);

    }
}
