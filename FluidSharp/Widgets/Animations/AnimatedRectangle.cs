using FluidSharp.Animations;
using FluidSharp.Layouts;
using FluidSharp.Paint;
using SkiaSharp;
using SkiaSharp.TextBlocks.Enum;
using System;
using System.Collections.Generic;
using System.Text;

namespace FluidSharp.Widgets
{

    public partial class Rectangle
    {
    
        public static Widget FillAnimated(IAnimation animation, Func<float, SKColor> getColor)
        {
            var value = animation.GetValue();
            var rectangle = Rectangle.Fill(getColor(value));
            return new AnimatedWidget(animation, rectangle, p => rectangle.BackgroundColor = getColor(p));
        }
    }

}
