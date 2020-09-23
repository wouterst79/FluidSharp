using FluidSharp.Layouts;
using SkiaSharp;
using SkiaSharp.TextBlocks.Enum;
using System;
using System.Collections.Generic;
using System.Text;

namespace FluidSharp.Widgets
{

    public class LinearGradient : Widget
    {


        public SKColor Color1;
        public SKColor Color2;
        public float Height;

        public LinearGradient(SKColor color1, SKColor color2, float height)
        {
            Color1 = color1;
            Color2 = color2;
            Height = height;
        }

        public override SKSize Measure(MeasureCache measureCache, SKSize boundaries)
        {
            return boundaries;
        }

        public override SKRect PaintInternal(LayoutSurface layoutsurface, SKRect rect)
        {

            rect = rect.WithHeight(Height);
            if (layoutsurface.Canvas != null)
            {

                using (
                var gradient = SKShader.CreateLinearGradient(
                    new SKPoint(0, rect.Top),
                    new SKPoint(0, rect.Bottom),
                    new SKColor[] { Color1, Color2 },
                    new float[] { 0, 1 },
                    SKShaderTileMode.Clamp))
                using (var colorpaint = new SKPaint() { Shader = gradient})
                {
                    layoutsurface.Canvas.DrawRect(rect, colorpaint);
                }

            }

            return rect;
        }

    }

}
