using FluidSharp.Touch;
using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Text;

namespace FluidSharp
{
    public interface ISkiaView
    {

        float Width { get; }
        float Height { get; }

        event EventHandler<PaintSurfaceEventArgs> PaintViewSurface;
        event EventHandler<TouchActionEventArgs> Touch;

        void InvalidatePaint();

    }
}
