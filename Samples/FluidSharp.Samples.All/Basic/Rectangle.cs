using FluidSharp.State;
using FluidSharp.Widgets;
using FluidSharp.Widgets.CrossPlatform;
using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Text;

namespace FluidSharp.Samples.All.Basic
{
    public class BackgroundColor : Sample
    {

        public override string Name => "Background Color";

        public override Widget MakeWidget(VisualState visualState)
        {
            return Rectangle.Fill(SKColors.Red);
        }

    }
}
