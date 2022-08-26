﻿using FluidSharp.Layouts;
using FluidSharp.Paint;
using SkiaSharp;
using SkiaSharp.TextBlocks.Enum;
using System;
using System.Collections.Generic;
using System.Text;

namespace FluidSharp.Widgets
{

    public class Rectangle : Widget
    {


        public Margins Margin;
        public SKSize MinimumSize;

        public SKColor BackgroundColor;
        public SKColor BorderColor;

        public bool FillHorizontal;
        public bool FillVertical;
        public bool Antialias;
        public float StrokeWidth;

        public Func<SKImageFilter>? ImageFilter;


        private Rectangle(bool fillHorizontal, bool fillVertical, SKColor backgroundcolor, SKColor bordercolor, Margins margin, SKSize minimumSize, bool antialias, float strokewidth)
        {
            FillHorizontal = fillHorizontal;
            FillVertical = fillVertical;
            BackgroundColor = backgroundcolor;
            BorderColor = bordercolor;
            Margin = margin;
            MinimumSize = minimumSize;
            Antialias = antialias;
            StrokeWidth = strokewidth;
        }

        public static Rectangle Sized(float width, float height, SKColor backgroundcolor, Margins margin = new Margins()) => new Rectangle(false, false, backgroundcolor, default, margin, new SKSize(width, height), width <= 2 || height <= 2, 0);

        public static Rectangle Horizontal(float height, SKColor backgroundcolor, Margins margin = new Margins()) => new Rectangle(true, false, backgroundcolor, default, margin, new SKSize(0, height), height <= 2, 0);

        public static Rectangle Vertical(float width, SKColor backgroundcolor, Margins margin = new Margins()) => new Rectangle(false, true, backgroundcolor, default, margin, new SKSize(width, 0), width <= 2, 0);

        public static Rectangle Fill(SKColor backgroundcolor, Margins margin = new Margins(), Func<SKImageFilter> imagefilter = default) => new Rectangle(true, true, backgroundcolor, default, margin, new SKSize(), false, 0) { ImageFilter = imagefilter };

        public static Rectangle Stroke(SKColor bordercolor, float strokewidth = 1, Margins margin = new Margins()) => new Rectangle(true, true, default, bordercolor, margin, new SKSize(), false, strokewidth);


        public override SKSize Measure(MeasureCache measureCache, SKSize boundaries)
        {

            var w = Margin.TotalX + MinimumSize.Width;
            var h = Margin.TotalY + MinimumSize.Height;

            return new SKSize(w, h);

        }

        public override SKRect PaintInternal(LayoutSurface layoutsurface, SKRect rect)
        {

            var withmargin = Margin.Shrink(rect, layoutsurface.Device.FlowDirection);

            // tight is default;
            var drawrect = new SKRect();
            if (FillHorizontal)
            {
                drawrect.Left = withmargin.Left;
                drawrect.Right = withmargin.Right;
            }
            else if (layoutsurface.Device.FlowDirection == FlowDirection.LeftToRight)
            {
                drawrect.Left = withmargin.Left;
                drawrect.Right = withmargin.Left + MinimumSize.Width;
            }
            else
            {
                drawrect.Left = withmargin.Right - MinimumSize.Width;
                drawrect.Right = withmargin.Right;
            }

            if (FillVertical)
            {
                drawrect.Top = rect.Top + Margin.Top;
                drawrect.Bottom = rect.Bottom - Margin.Bottom;
            }
            else
            {
                drawrect.Top = rect.Top + Margin.Top;
                drawrect.Bottom = rect.Top + Margin.Top + MinimumSize.Height;
            }


            if (layoutsurface.Canvas != null)
            {

                if (BackgroundColor != null && BackgroundColor.Alpha != 0)
                {
                    var paint = PaintCache.GetBackgroundPaint(BackgroundColor, Antialias, ImageFilter);
                    layoutsurface.Canvas.DrawRect(drawrect, paint);
                }

                if (BorderColor != null && BorderColor.Alpha != 0)
                {
                    layoutsurface.Canvas.DrawRect(drawrect, PaintCache.GetBorderPaint(BorderColor, Antialias, StrokeWidth));
                }

            }

            return Margin.Grow(drawrect, layoutsurface.Device.FlowDirection);
        }

        public static Widget? Fill(object iOSLightGrey) => throw new NotImplementedException();
    }

}
