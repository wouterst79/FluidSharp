using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Text;
using SKPaint1 = SkiaSharp.SKPaint;

namespace FluidSharp.Paint
{
    public static class PaintCache
    {

        private static SKPaint1? backgroundPaintT;
        private static SKPaint1? backgroundPaintF;
        public static SKPaint1 GetBackgroundPaint(SKColor backgroundColor)
        {
            var paint = backgroundPaintT ??= new SKPaint1() { IsAntialias = true };
            paint.Color = backgroundColor;
            return paint;
        }

        public static SKPaint1 GetBackgroundPaint(SKColor backgroundColor, bool antiAlias)
        {
            var paint = antiAlias ?
                                        backgroundPaintT ??= new SKPaint1() { IsAntialias = true } :
                                        backgroundPaintF ??= new SKPaint1();
            paint.Color = backgroundColor;
            return paint;
        }

        private static SKPaint1? imageFilterPaint;
        public static SKPaint1 GetBackgroundPaint(SKColor backgroundColor, bool antialias, Func<SKImageFilter>? imageFilter)
        {
            if (imageFilter is null)
            {
                var paint = antialias ?
                                            backgroundPaintT ??= new SKPaint1() { IsAntialias = true } :
                                            backgroundPaintF ??= new SKPaint1();
                paint.Color = backgroundColor;
                return paint;
            }
            else
            {
                var paint = imageFilterPaint ??= new SKPaint1();
                paint.Color = backgroundColor;
                paint.IsAntialias = antialias;
                return paint;
            }
        }

        private static SKPaint1? borderPaintT;
        private static SKPaint1? borderPaintF;
        //public static SKPaint1 GetBorderPaint(SKColor borderColor)
        //{
        //    var borderPaint = borderPaintT ??= new SKPaint1() { IsAntialias = true };
        //    borderPaint.Color = borderColor;
        //    return borderPaint;
        //}

        public static SKPaint1 GetBorderPaint(SKColor borderColor, float strokeWidth)
        {
            var paint = borderPaintT ??= new SKPaint1() { IsAntialias = true };
            paint.Color = borderColor;
            paint.StrokeWidth = strokeWidth;
            return paint;
        }

        public static SKPaint1 GetBorderPaint(SKColor borderColor, bool antiAlias, float strokeWidth)
        {
            var paint = antiAlias ?
                                        borderPaintT ??= new SKPaint1() { IsAntialias = true } :
                                        borderPaintF ??= new SKPaint1();
            paint.Color = borderColor;
            paint.StrokeWidth = strokeWidth;
            return paint;
        }


        private static SKPaint1? textPaint;
        public static SKPaint1 GetTextPaint(SKColor color, float textSize)
        {
            var paint = textPaint ??= new SKPaint1();
            paint.Color = color;
            paint.TextSize = textSize;
            return paint;
        }

        private static SKPaint1? shaderPaint;
        public static SKPaint1 GetShaderPaint(SKShader shader)
        {
            var paint = shaderPaint ??= new SKPaint1();
            paint.Shader = shader;
            return paint;
        }

        private static SKPaint1? imagePaint;
        private static SKPaint1? imageOpacityPaint;

        public static SKPaint1 GetImagePaint()
        {
            var paint = imagePaint ??= new SKPaint1() { FilterQuality = SKFilterQuality.High };
            return paint;
        }

        public static SKPaint1 GetImagePaint(float opacity)
        {
            if (opacity >= 1)
            {
                var paint = imagePaint ??= new SKPaint1() { FilterQuality = SKFilterQuality.High };
                return paint;
            }
            else
            {
                var paint = imageOpacityPaint ??= new SKPaint1() { FilterQuality = SKFilterQuality.High };
                paint.Color = SKColors.White.WithOpacity(opacity);
                return paint;
            }
        }


        private static SKPaint1? opacityPaint;
        public static SKPaint1 GetOpacityPaint(float opacity)
        {
            var paint = opacityPaint ??= new SKPaint1();
            paint.Color = SKColors.White.WithOpacity(opacity);
            return paint;
        }

    }
}
