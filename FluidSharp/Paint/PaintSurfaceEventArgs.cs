using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Text;

namespace FluidSharp
{
    public class PaintSurfaceEventArgs
    {
        public SKCanvas Canvas;
        public float Width;
        public float Height;
        public SKSurface Surface;
        public SKImageInfo Info;

        public PaintSurfaceEventArgs(SKCanvas canvas, float width, float height, SKSurface surface, SKImageInfo info)
        {
            Canvas = canvas;
            Width = width;
            Height = height;
            Surface = surface;
            Info = info;
        }

    }
}
