using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Text;

namespace FluidSharp
{
    public interface IImageSource
    {

        SKImage GetImage(string name);

    }

}
