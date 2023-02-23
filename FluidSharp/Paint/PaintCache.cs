using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Text;

namespace FluidSharp.Paint
{
    public static class PaintCache
    {

        private static SKPaint? backgroundPaintT;
        private static SKPaint? backgroundPaintF;
        public static SKPaint GetBackgroundPaint(SKColor backgroundColor)
        {
            var paint = backgroundPaintT ??= new SKPaint() { IsAntialias = true };
            paint.Color = backgroundColor;
            return paint;
        }

        public static SKPaint GetBackgroundPaint(SKColor backgroundColor, bool antiAlias)
        {
            var paint = antiAlias ?
                                        backgroundPaintT ??= new SKPaint() { IsAntialias = true } :
                                        backgroundPaintF ??= new SKPaint();
            paint.Color = backgroundColor;
            return paint;
        }

        private static SKPaint? imageFilterPaint;
        public static SKPaint GetBackgroundPaint(SKColor backgroundColor, bool antialias, Func<SKImageFilter>? imageFilter)
        {
            if (imageFilter is null)
            {
                var paint = antialias ?
                                            backgroundPaintT ??= new SKPaint() { IsAntialias = true } :
                                            backgroundPaintF ??= new SKPaint();
                paint.Color = backgroundColor;
                return paint;
            }
            else
            {
                var paint = imageFilterPaint ??= new SKPaint();
                paint.Color = backgroundColor;
                paint.IsAntialias = antialias;
                paint.ImageFilter = imageFilter();
                return paint;
            }
        }

        private static SKPaint? borderPaintT;
        private static SKPaint? borderPaintF;
        //public static SKPaint GetBorderPaint(SKColor borderColor)
        //{
        //    var borderPaint = borderPaintT ??= new SKPaint() { IsAntialias = true };
        //    borderPaint.Color = borderColor;
        //    return borderPaint;
        //}

        public static SKPaint GetBorderPaint(SKColor borderColor, float strokeWidth)
        {
            var paint = borderPaintT ??= new SKPaint() { IsAntialias = true, IsStroke = true };
            paint.Color = borderColor;
            paint.StrokeWidth = strokeWidth;
            return paint;
        }

        public static SKPaint GetBorderPaint(SKColor borderColor, bool antiAlias, float strokeWidth)
        {
            var paint = antiAlias ?
                                        borderPaintT ??= new SKPaint() { IsAntialias = true, IsStroke = true } :
                                        borderPaintF ??= new SKPaint() { IsStroke = true };
            paint.Color = borderColor;
            paint.StrokeWidth = strokeWidth;
            return paint;
        }


        private static SKPaint? strokeCapPaint;
        public static SKPaint GetStrokeCapPaint(SKColor color, bool antiAlias, float strokeWidth, SKStrokeCap strokeCap)
        {
            var paint = strokeCapPaint ??= new SKPaint() { IsStroke = true };
            paint.Color = color;
            paint.IsAntialias = antiAlias;
            paint.StrokeWidth = strokeWidth;
            paint.StrokeCap = strokeCap;
            return paint;
        }

        private static SKPaint? textPaint;
        public static SKPaint GetTextPaint(SKColor color, float textSize)
        {
            var paint = textPaint ??= new SKPaint();
            paint.Color = color;
            paint.TextSize = textSize;
            return paint;
        }

        private static SKPaint? shaderPaint;
        public static SKPaint GetShaderPaint(SKShader shader)
        {
            var paint = shaderPaint ??= new SKPaint();
            paint.Shader = shader;
            return paint;
        }

        private static SKPaint? backgroundPaint;
        private static SKPaint? imagePaint;
        private static SKPaint? imageOpacityPaint;

        public static SKPaint GetBackgroundPaint()
        {
            var paint = backgroundPaint ??= new SKPaint() { FilterQuality = SKFilterQuality.Low };
            return paint;
        }

        public static SKPaint GetImagePaint()
        {
            var paint = imagePaint ??= new SKPaint() { FilterQuality = SKFilterQuality.High };
            return paint;
        }

        public static SKPaint GetImagePaint(float opacity)
        {
            if (opacity >= 1)
            {
                var paint = imagePaint ??= new SKPaint() { FilterQuality = SKFilterQuality.High };
                return paint;
            }
            else
            {
                var paint = imageOpacityPaint ??= new SKPaint() { FilterQuality = SKFilterQuality.High };
                paint.Color = SKColors.White.WithOpacity(opacity);
                return paint;
            }
        }


        private static SKPaint? opacityPaint;
        public static SKPaint GetOpacityPaint(float opacity)
        {
            var paint = opacityPaint ??= new SKPaint();
            paint.Color = SKColors.White.WithOpacity(opacity);
            return paint;
        }

    }
}
