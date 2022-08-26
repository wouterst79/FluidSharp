﻿using FluidSharp.Layouts;
using FluidSharp.Paint;
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

        public Func<SKImageFilter>? ImageFilter;

        public Widget? ClippedContents;

        public RoundedRectangle(float cornerRadius, SKColor backgroundColor, SKColor borderColor, Widget clippedContents = null)
        {
            CornerRadius = cornerRadius;
            BackgroundColor = backgroundColor;
            BorderColor = borderColor;
            ClippedContents = clippedContents;
        }

        public override SKSize Measure(MeasureCache measureCache, SKSize boundaries)
        {

            if (ClippedContents == null)
                return new SKSize(CornerRadius * 3, CornerRadius * 3);
            else
                return ClippedContents.Measure(measureCache, boundaries);
        }

        public override SKRect PaintInternal(LayoutSurface layoutsurface, SKRect rect)
        {

            using (var rrect = new SKRoundRect(rect, CornerRadius, CornerRadius))
            {

                if (layoutsurface.Canvas != null)
                {
                    if (BackgroundColor != null && BackgroundColor.Alpha != 0)
                    {
                        var paint = PaintCache.GetBackgroundPaint(BackgroundColor, true, ImageFilter);
                        layoutsurface.Canvas.DrawRoundRect(rrect, paint);
                    }

                    if (BorderColor != null && BorderColor.Alpha != 0)
                    {
                        layoutsurface.Canvas.DrawRoundRect(rrect, PaintCache.GetBorderPaint(BorderColor, true, 1));
                    }
                }

                if (ClippedContents != null)
                {

                    var clippath = new SKPath();

                    clippath.AddRoundRect(rrect);
                    layoutsurface.ClipPath(clippath);

                    layoutsurface.Paint(ClippedContents, rect);

                    layoutsurface.ResetPathClip();

                }
            }

            return rect;
        }

    }
}
