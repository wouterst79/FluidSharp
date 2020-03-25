using FluidSharp.Layouts;
using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Text;

namespace FluidSharp.Widgets
{
    public class Shape : Widget
    {

        public SKPath Path;
        public SKColor BackgroundColor;
        public SKColor BorderColor;

        public Shape(SKPath path, SKColor backgroundColor, SKColor borderColor)
        {
            Path = path;
            BackgroundColor = backgroundColor;
            BorderColor = borderColor;
        }

        public override SKSize Measure(MeasureCache measureCache, SKSize boundaries)
        {
            return boundaries;
        }

        public override SKRect PaintInternal(LayoutSurface layoutsurface, SKRect rect)
        {

            var canvas = layoutsurface.Canvas;
            if (canvas != null)
            {

                var bounds = Path.Bounds;
                using (var drawpath = new SKPath(Path))
                {

                    drawpath.Transform(SKMatrix.MakeScale(rect.Width / bounds.Width, rect.Height / bounds.Height));
                    drawpath.Transform(SKMatrix.MakeTranslation(rect.Left, rect.Top));

                    if (BackgroundColor != null && BackgroundColor.Alpha != 0)
                        using (var paint = new SKPaint() { Color = BackgroundColor, IsAntialias = true })
                            canvas.DrawPath(drawpath, paint);

                    if (BorderColor != null && BorderColor.Alpha != 0)
                        using (var paint = new SKPaint() { Color = BorderColor, IsStroke = true, IsAntialias = true })
                            canvas.DrawPath(drawpath, paint);

                }
            }

            return rect;
        }


    }
}
