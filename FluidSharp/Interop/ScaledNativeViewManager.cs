using FluidSharp.Widgets.Native;
using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Text;

namespace FluidSharp.Interop
{
    public class ScaledNativeViewManager : INativeViewManager
    {

        public SKPoint Factor;
        public INativeViewManager InnerManager;

        public ScaledNativeViewManager(SKPoint factor, INativeViewManager innerManager)
        {
            Factor = factor;
            InnerManager = innerManager;
        }

        public SKSize Measure(NativeViewWidget nativeViewWidget, SKSize boundaries)
        {
            var scaledboundaries = new SKSize(boundaries.Width * Factor.X, boundaries.Height * Factor.Y);
            var childsize = InnerManager.Measure(nativeViewWidget, scaledboundaries);
            return new SKSize(childsize.Width / Factor.X, childsize.Height / Factor.Y);
        }

        public void PaintStarted() => InnerManager.PaintStarted();

        public void UpdateNativeView(NativeViewWidget nativeViewWidget, SKRect rect)
        {
            var scaled = new SKRect(rect.Left * Factor.X, rect.Top * Factor.Y, rect.Right * Factor.X, rect.Bottom * Factor.Y);
            InnerManager.UpdateNativeView(nativeViewWidget, scaled);
        }

        public void PaintCompleted() => InnerManager.PaintCompleted();

    }
}
