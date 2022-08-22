using FluidSharp.Layouts;
using FluidSharp.Paint;
using SkiaSharp;
using SkiaSharp.TextBlocks.Enum;
using System;
using System.Collections.Generic;
using System.Text;

namespace FluidSharp.Widgets
{

    public class HorizontalGradient : Widget
    {


        public SKColor NearColor;
        public SKColor FarColor;
        public float Width;
        public float Spacing;
        public Widget Far;

        public HorizontalGradient(SKColor nearColor, SKColor farColor, float width)
        {
            NearColor = nearColor;
            FarColor = farColor;
            Width = width;
        }

        public override SKSize Measure(MeasureCache measureCache, SKSize boundaries)
        {
            return new SKSize(Width, 0);
        }

        public override SKRect PaintInternal(LayoutSurface layoutsurface, SKRect rect)
        {

            if (layoutsurface.Canvas != null)
            {

                if (layoutsurface.IsRtl)
                {

                    var drawrect = new SKRect(rect.Left, rect.Top, rect.Left + Width, rect.Bottom);
                    using (var gradient = SKShader.CreateLinearGradient(
                        new SKPoint(drawrect.Left, 0),
                        new SKPoint(drawrect.Right, 0),
                        new SKColor[] { FarColor, NearColor },
                        new float[] { 1, 0 },
                        SKShaderTileMode.Clamp))
                        layoutsurface.Canvas.DrawRect(drawrect, PaintCache.GetShaderPaint(gradient));

                }
                else
                {

                    var drawrect = new SKRect(rect.Right - Width, rect.Top, rect.Right, rect.Bottom);
                    using (var gradient = SKShader.CreateLinearGradient(
                        new SKPoint(drawrect.Left, 0),
                        new SKPoint(drawrect.Right, 0),
                        new SKColor[] { NearColor, FarColor },
                        new float[] { 0, 1 },
                        SKShaderTileMode.Clamp))
                        layoutsurface.Canvas.DrawRect(drawrect, PaintCache.GetShaderPaint(gradient));

                }

            }

            return rect;
        }

    }

}
