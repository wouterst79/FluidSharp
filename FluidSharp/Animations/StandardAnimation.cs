//#if DEBUG
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

        public enum Direction
        {
            None,
            Near,
            Top,
            Far,
            Bottom
        }

        private static Widget Translate(Widget widget, Direction direction, float w, float h, float pct)
        {
            if (direction == Direction.None) return widget;
            float dx = 0; float dy = 0;
            switch (direction)
            {
                case Direction.Near: dx = -w * pct; break;
                case Direction.Top: dy = -h * pct; break;
                case Direction.Far: dx = w * pct; break;
                case Direction.Bottom: dy = h * pct; break;
            }
            return new Translate(dx, dy, widget);
        }

        private static float FullWidth = 400;

        public static Widget FadeIn(this Animation animation, Text text, Direction fromDirection = Direction.Bottom)
        {
            if (animation.Completed) return text;
            var pct = animation.GetValue();
            var textblock = text.TextBlock;
            textblock.Color = textblock.Color.WithOpacity(pct);
            return Translate(text, fromDirection, FullWidth, textblock.Font.TextSize, 1 - pct);
        }


        public static Widget FadeIn(this Animation animation, Widget widget, float height, Direction fromDirection = Direction.Bottom)
        {
            if (widget is Text text) return FadeIn(animation, text, fromDirection);
            if (animation.Completed) return widget;
            var pct = animation.GetValue();
            return Translate(new Opacity(pct, widget), fromDirection, FullWidth, height, 1 - pct);
        }

        public static Widget FadeIn(this Animation animation, Picture picture, Direction fromDirection = Direction.Bottom)
        {
            if (animation.Completed) return picture;
            var pct = animation.GetValue();
            picture = picture.WithOpacity(pct);
            return Translate(picture, fromDirection, picture.Size.Width, picture.Size.Height, 1 - pct);
        }

        public static Widget FlyOut(this Animation animation, Text text, Direction toDirection = Direction.Near)
        {
            if (animation.Completed)
            {
                text.TextBlock.Color = SKColors.Transparent;
                return text;
            }
            var pct = animation.GetValue();
            var textblock = text.TextBlock;
            textblock.Color = textblock.Color.WithOpacity(1 - pct);
            return Translate(text, toDirection, FullWidth, textblock.Font.TextSize, pct);
        }

        public static Widget FlyOut(this Animation animation, Widget widget, float height, Direction toDirection = Direction.Near)
        {
            var pct = animation.GetValue();
            widget = new Opacity(1 - pct, widget);
            return new AnimatedWidget(animation, Translate(widget, toDirection, FullWidth, height, pct));
        }


        public static Widget FlyOut(this Animation animation, Picture picture, Direction toDirection = Direction.Near)
        {
            if (animation.Completed)
            {
                return picture.WithOpacity(0);
            }
            var pct = animation.GetValue();
            picture = picture.WithOpacity(1 - pct);
            return Translate(picture, toDirection, picture.Size.Width, picture.Size.Height, pct);
        }

    }
}
