﻿using FluidSharp.Layouts;
using SkiaSharp;
using SkiaSharp.TextBlocks.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FluidSharp.Widgets
{
    public class Flow : Widget
    {

        public bool Justify;
        public float Spacing;

        public List<Widget> Children = new List<Widget>();


        public override SKSize Measure(MeasureCache measureCache, SKSize boundaries)
        {
            var calculated = Layout(new SKRect(0, 0, boundaries.Width, boundaries.Height), null, measureCache);
            return new SKSize(calculated.Width, calculated.Height);
        }

        public override SKRect PaintInternal(LayoutSurface layoutsurface, SKRect rect)
        {
            return Layout(rect, layoutsurface, layoutsurface.MeasureCache);
        }

        private SKRect Layout(SKRect rect, LayoutSurface layoutsurface, MeasureCache measureCache)
        {

            var boundaries = new SKSize(rect.Width, 0);

            var width = rect.Width;
            var threshold = width * .9f;

            var line = new List<(Widget view, SKSize size)>();
            var linewidth = -Spacing;
            var maxlinewidth = 0.0f;
            var y = rect.Top;

            var any = false;
            if (Children != null)
                foreach (var child in Children)
                    if (child != null)
                    {

                        var measured = child.Measure(measureCache, boundaries);

                        if (line.Count > 0 && linewidth + measured.Width > threshold)
                            LayoutLine();

                        line.Add((child, measured));
                        linewidth += measured.Width + Spacing;

                        if (maxlinewidth < linewidth)
                            maxlinewidth = linewidth;

                        any = true;

                    }

            if (line.Count > 0)
                LayoutLine();

            if (any)
                y -= Spacing;

            if (Justify)
                return new SKRect(rect.Left, rect.Top, rect.Left + width, y);
            else if (layoutsurface.Device.FlowDirection == FlowDirection.LeftToRight)
                return new SKRect(rect.Left, rect.Top, rect.Left + maxlinewidth, y);
            else
                return new SKRect(rect.Right - maxlinewidth, rect.Top, rect.Right, y);

            void LayoutLine()
            {

                var widthfactor = 1f;

                if (Justify)
                {
                    var totalreqwidth = line.Sum(l => l.size.Width);
                    //if (double.IsInfinity(width)) width = totalreqwidth + (line.Count - 1) * ButtonMargin;
                    widthfactor = (width - (line.Count - 1) * Spacing) / totalreqwidth;

                    if (widthfactor < 1)
                    {
#if DEBUG
                        Console.WriteLine($"warning: flow width factor < 1: {widthfactor}");
#endif
                        widthfactor = 1;
                    }
                }

                var h = line.Max(l => l.size.Height);

                if (layoutsurface != null)
                {

                    if (layoutsurface.Device.FlowDirection == FlowDirection.LeftToRight)
                    {

                        var x = rect.Left;
                        foreach (var set in line)
                        {

                            var (child, sizerequest) = set;
                            var w = sizerequest.Width * widthfactor;
                            var childrect = new SKRect(x, y, x + w, y + h);
                            layoutsurface.Paint(child, childrect);

                            x += Spacing + w;

                        }

                    }
                    else
                    {

                        var x = rect.Right;
                        foreach (var set in line)
                        {

                            var (child, sizerequest) = set;
                            var w = sizerequest.Width * widthfactor;
                            var childrect = new SKRect(x - w, y, x, y + h);
                            layoutsurface.Paint(child, childrect);

                            x -= Spacing + w;

                        }

                    }

                }

                linewidth = -Spacing;
                y += h + Spacing;

                line.Clear();

            }
        }

    }
}