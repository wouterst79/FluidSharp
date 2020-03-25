﻿#if DEBUG
//#define DEBUGCONTAINER
#endif

using FluidSharp.Layouts;
using SkiaSharp;
using SkiaSharp.TextBlocks.Enum;
using System;
using System.Collections.Generic;
using System.Text;

namespace FluidSharp.Widgets
{

    public enum ContainerLayout
    {

        /// <summary>
        /// Wrap uses the child measure for both measure and paint
        /// </summary>
        Wrap,

        /// <summary>
        /// Fill returns tight boundaries on measure, and fills up the paint region completely
        /// </summary>
        Fill,
        FillHorizontal,

        /// <summary>
        /// Expand uses the full measure boundaries, and the paint region
        /// </summary>
        Expand,
        ExpandHorizontal

    }

    public class Container : Widget
    {

        public Margins Margin;
        public SKSize MinimumSize;

        public bool FillHorizontal;
        public bool FillVertical;

        public bool ExpandHorizontal;
        public bool ExpandVertical;

        public List<Widget> Children = new List<Widget>();


        public Container(ContainerLayout layout)
        {

            ExpandHorizontal = layout == ContainerLayout.Expand || layout == ContainerLayout.ExpandHorizontal;
            ExpandVertical = layout == ContainerLayout.Expand;

            FillHorizontal = layout == ContainerLayout.Fill || layout == ContainerLayout.FillHorizontal || ExpandHorizontal;
            FillVertical = layout == ContainerLayout.Fill || ExpandVertical;

        }


        public static Container Make(ContainerLayout layout, Margins margins, Widget child)
        {
            return new Container(layout) { Children = new List<Widget>() { child }, Margin = margins };
        }

        public override SKSize Measure(MeasureCache measureCache, SKSize boundaries)
        {

            if (ExpandHorizontal && ExpandVertical)
                return boundaries;

            var childboundaries = new SKSize(
                                                boundaries.Width - Margin.TotalX,
                                                boundaries.Height - Margin.TotalY
                                            );

            var result = MinimumSize;
            if (Children != null)
                foreach (var child in Children)
                    if (child != null)
                    {
                        var childsize = child.Measure(measureCache, childboundaries);
                        if (result.Width < childsize.Width)
                            result.Width = childsize.Width;
                        if (result.Height < childsize.Height)
                            result.Height = childsize.Height;
                    }

            result.Width += Margin.TotalX;
            result.Height += Margin.TotalY;

            if (ExpandHorizontal) result.Width = boundaries.Width;
            if (ExpandVertical) result.Height = boundaries.Height;

            return result;

        }

        public override SKRect PaintInternal(LayoutSurface layoutsurface, SKRect rect)
        {

            var size = Measure(layoutsurface.MeasureCache, new SKSize(rect.Width, rect.Height));


            // apply size
            var drawrect = new SKRect();
            if (FillHorizontal)
            {
                drawrect.Left = rect.Left;
                drawrect.Right = rect.Right;
            }
            else if (layoutsurface.Device.FlowDirection == FlowDirection.LeftToRight)
            {
                drawrect.Left = rect.Left;
                drawrect.Right = rect.Left + size.Width;
            }
            else
            {
                drawrect.Left = rect.Right - size.Width;
                drawrect.Right = rect.Right;
            }

            if (FillVertical)
            {
                drawrect.Top = rect.Top;
                drawrect.Bottom = rect.Bottom;
            }
            else
            {
                drawrect.Top = rect.Top;
                drawrect.Bottom = rect.Top + size.Height;
            }

            // apply margins
            var childrect = Margin.Apply(drawrect, layoutsurface.Device.FlowDirection);

            // paint children
            if (Children != null)
                foreach (var child in Children)
                    if (child != null)
                        layoutsurface.Paint(child, childrect);

#if DEBUGCONTAINER
            layoutsurface.DebugRect(drawrect, SKColors.Blue.WithAlpha(128));
#endif

            return drawrect;
        }

    }

}