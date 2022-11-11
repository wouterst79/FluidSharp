using SkiaSharp;
using SkiaSharp.TextBlocks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AColor = Android.Graphics.Color;

namespace FluidSharp.Views.Android.NativeViews
{
    public static class NativeViewExtensions
    {


        public static AColor ToAndroid(this SKColor self)
        {
            return new AColor(self.Red, self.Green, self.Blue, self.Alpha);
        }

    }
}