using FluidSharp.Widgets;
using SkiaSharp;
using SkiaSharp.TextBlocks;
using System;
using System.Collections.Generic;
using System.Text;

namespace FluidSharp.Animations
{
    public static class StandardAnimation
    {

        public static Widget FadeIn(this Animation animation, Text text)
        {

            if (animation.Completed)
                return text;

            var pct = animation.GetValue();

            var textblock = text.TextBlock;
            var dy = textblock.Font.TextSize * (1 - pct);
            text.TextBlock.Color = text.TextBlock.Color.WithOpacity(pct);

            return new Translate(0, dy, text);

        }

        public static Widget FlyOut(this Animation animation, Text text)
        {

            if (animation.Completed)
            {
                text.TextBlock.Color = SKColors.Transparent;
                return text;
            }

            var pct = animation.GetValue();

            var textblock = text.TextBlock;
            var dx = -300 * pct;
            text.TextBlock.Color = text.TextBlock.Color.WithOpacity(1 - pct);

            return new Translate(dx, 0, text);

        }

        public static Widget FadeOut(this Animation animation, Picture picture)
        {

            if (animation.Completed)
            {
                picture.Opacity = 0;
                return picture;
            }

            var pct = animation.GetValue();
            var dx = -picture.Size.Width * pct;
            picture.Opacity = (1 - pct);

            return new Translate(dx, 0, picture);

        }


    }
}
