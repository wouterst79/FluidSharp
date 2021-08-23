using FluidSharp.State;
using FluidSharp.Widgets;
using FluidSharp.Widgets.CrossPlatform;
using SkiaSharp;
using SkiaSharp.TextBlocks;
using System;
using System.Collections.Generic;
using System.Text;
using Font = SkiaSharp.TextBlocks.Font;

namespace FluidSharp.Samples.All.Basic
{
    public class HelloWorld : Sample
    {

        public override string Name => "Hello world";

        public override Widget MakeWidget(VisualState visualState)
        {
            return new Text(new Font(14), SKColors.Black, "Hello World!");
        }

    }
}
