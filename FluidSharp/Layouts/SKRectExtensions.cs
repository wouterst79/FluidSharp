using FluidSharp.Widgets;
using SkiaSharp;
using SkiaSharp.TextBlocks.Enum;
using System;
using System.Collections.Generic;
using System.Text;

namespace FluidSharp.Layouts
{
    public static class SKRectExtensions
    {

        public static SKRect Move(this SKRect rect, SKPoint delta, bool minusx)
        {
            if (minusx)
                return new SKRect(rect.Left - delta.X, rect.Top + delta.Y, rect.Right - delta.X, rect.Bottom + delta.Y);
            else
                return new SKRect(rect.Left + delta.X, rect.Top + delta.Y, rect.Right + delta.X, rect.Bottom + delta.Y);
        }


        public static SKRect Scale(this SKRect rect, ScaleMode scaleMode, SKSize aspect)
        {

            if (scaleMode == ScaleMode.Strech) return rect;

            var w = rect.Width;
            var h = rect.Height;

            var wfromh = h * aspect.Width / aspect.Height;
            var hfromw = w * aspect.Height / aspect.Width;

            if (scaleMode == ScaleMode.Fit && wfromh < w || scaleMode == ScaleMode.Fill && wfromh > w)
                return new SKRect(rect.MidX - wfromh / 2, rect.Top, rect.MidX + wfromh / 2, rect.Bottom);
            else
                return new SKRect(rect.Left, rect.MidY - hfromw / 2, rect.Right, rect.MidY + hfromw / 2);
        }

        public static SKRect Fit(this SKRect rect, SKSize size, FlowDirection flowDirection)
        {
            if (flowDirection != FlowDirection.RightToLeft)
                return new SKRect(rect.Left, rect.Top, rect.Left + size.Width, rect.Top + size.Height);
            else
                return new SKRect(rect.Right - size.Width, rect.Top, rect.Right, rect.Top + size.Height);
        }

        public static SKRect WithHeight(this SKRect rect, float height)
        {
            return new SKRect(rect.Left, rect.Top, rect.Right, rect.Top + height);
        }

        public static SKRect FromBottomHeight(this SKRect rect, float height)
        {
            return new SKRect(rect.Left, rect.Bottom - height, rect.Right, rect.Bottom);
        }

        public static SKRect Center(this SKRect rect, SKSize size)
        {
            return new SKRect(rect.MidX - size.Width / 2,
                              rect.MidY - size.Height / 2,
                              rect.MidX + size.Width / 2,
                              rect.MidY + size.Height / 2);
        }

        public static SKRect TopCenter(this SKRect rect, SKSize size)
        {
            return new SKRect(rect.MidX - size.Width / 2,
                              rect.Top,
                              rect.MidX + size.Width / 2,
                              rect.Top + size.Height);
        }

        public static SKRect HorizontalAlign(this SKRect rect, SKSize size, HorizontalAlignment horizontalAlignment, FlowDirection flowDirection)
        {
            if (horizontalAlignment == HorizontalAlignment.Center)
            {
                return new SKRect(rect.MidX - size.Width / 2,
                                  rect.Top,
                                  rect.MidX + size.Width / 2,
                                  rect.Top + size.Height);
            }

            if ((horizontalAlignment == HorizontalAlignment.Near && flowDirection == FlowDirection.LeftToRight)
              || (horizontalAlignment == HorizontalAlignment.Far && flowDirection == FlowDirection.RightToLeft))
            {
                // left
                return new SKRect(rect.Left, rect.Top, rect.Left + size.Width, rect.Top + size.Height);
            }
            else
            {
                // right
                return new SKRect(rect.Right - size.Width, rect.Top, rect.Right, rect.Top + size.Height);
            }
        }

        public static SKRect HorizontalAlign(this SKRect rect, SKSize size, HorizontalAlignment horizontalAlignment, FlowDirection flowDirection, float XMargin, float YMargin)
        {
            if (horizontalAlignment == HorizontalAlignment.Center)
            {
                return new SKRect(rect.MidX - size.Width / 2,
                                  rect.Top,
                                  rect.MidX + size.Width / 2,
                                  rect.Top + size.Height);
            }

            if ((horizontalAlignment == HorizontalAlignment.Near && flowDirection == FlowDirection.LeftToRight)
              || (horizontalAlignment == HorizontalAlignment.Far && flowDirection == FlowDirection.RightToLeft))
            {
                // left
                return new SKRect(rect.Left + XMargin, rect.Top + YMargin, rect.Left + size.Width + XMargin, rect.Top + size.Height + YMargin);
            }
            else
            {
                // right
                return new SKRect(rect.Right - size.Width - XMargin, rect.Top + YMargin, rect.Right - XMargin, rect.Top + size.Height + YMargin);
            }
        }

    }
}
