using FluidSharp.Layouts;
using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Text;

namespace FluidSharp.Widgets
{
    public class RoundedRectangle : Widget
    {

        public float CornerRadius;
        public SKColor BackgroundColor;
        public SKColor BorderColor;


        public RoundedRectangle(float cornerRadius, SKColor backgroundColor, SKColor borderColor)
        {
            CornerRadius = cornerRadius;
            BackgroundColor = backgroundColor;
            BorderColor = borderColor;
        }

        public override SKSize Measure(MeasureCache measureCache, SKSize boundaries)
        {
            return new SKSize(CornerRadius * 3, CornerRadius * 3);
        }

        public override SKRect PaintInternal(LayoutSurface layoutsurface, SKRect rect)
        {

            if (layoutsurface.Canvas != null)
                using (var rrect = new SKRoundRect(rect, CornerRadius, CornerRadius))
                {
                    if (BackgroundColor != null && BackgroundColor.Alpha != 0)
                        using (var paint = new SKPaint() { Color = BackgroundColor, IsAntialias = true })
                            layoutsurface.Canvas.DrawRoundRect(rrect, paint);

                    if (BorderColor != null && BorderColor.Alpha != 0)
                        using (var paint = new SKPaint() { Color = BorderColor, IsStroke = true, IsAntialias = true })
                            layoutsurface.Canvas.DrawRoundRect(rrect, paint);
                }

            return rect;
        }

    }
}
