//#if DEBUG
using FluidSharp.Widgets;
using FluidSharp.Widgets.Animations;
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

        private static Widget Translate(Widget widget, Direction direction, float w, float h, Animation animation, Action<float>? additionalAnimation = null)
        {
            if (direction == Direction.None) return widget;
            var translate = new Translate(0, 0, widget);
            return new AnimatedWidget(animation, translate, pct =>
            {
                float dx = 0; float dy = 0;
                switch (direction)
                {
                    case Direction.Near: dx = -w * pct; break;
                    case Direction.Top: dy = -h * pct; break;
                    case Direction.Far: dx = w * pct; break;
                    case Direction.Bottom: dy = h * pct; break;
                }
                translate.Translation = new SKPoint(dx, dy);
                additionalAnimation?.Invoke(pct);
            });
        }

        private static float FullWidth = 400;

        public static Widget FadeIn(this Animation animation, Text text, Direction fromDirection = Direction.Bottom)
        {
            var textblock = text.TextBlock;
            var color = text.TextBlock.Color;
            return Translate(text, fromDirection, FullWidth, textblock.Font.TextSize, animation.Inverse(), pct =>
            {
                textblock.Color = color.WithOpacity(1 - pct);
            });
        }


        public static Widget FadeIn(this Animation animation, Widget widget, float height, Direction fromDirection = Direction.Bottom)
        {
            if (widget is Text text) return FadeIn(animation, text, fromDirection);
            return Translate(new FadeInElement(animation, widget), fromDirection, FullWidth, height, animation.Inverse());
        }

        public static Widget FadeIn(this Animation animation, Picture picture, Direction fromDirection = Direction.Bottom)
        {
            picture = picture.WithOpacity(1);
            return Translate(picture, fromDirection, picture.Size.Width, picture.Size.Height, animation.Inverse(), (pct) =>
            {
                picture.Opacity = 1 - pct;
            });
        }

        public static Widget FlyOut(this Animation animation, Text text, Direction toDirection = Direction.Near)
        {
            var textblock = text.TextBlock;
            var color = text.TextBlock.Color;
            return Translate(text, toDirection, FullWidth, textblock.Font.TextSize, animation, pct =>
            {
                textblock.Color = color.WithOpacity(1 - pct);
            });
        }

        public static Widget FlyOut(this Animation animation, Widget widget, float height, Direction toDirection = Direction.Near)
        {
            widget = new FadeInElement(animation.Inverse(), widget);
            return new AnimatedWidget(animation, Translate(widget, toDirection, FullWidth, height, animation));
        }


        public static Widget FlyOut(this Animation animation, Picture picture, Direction toDirection = Direction.Near)
        {
            picture = picture.WithOpacity(1);
            return Translate(picture, toDirection, picture.Size.Width, picture.Size.Height, animation, (pct) =>
            {
                picture.Opacity = (1 - pct);
            });
        }

    }
}
