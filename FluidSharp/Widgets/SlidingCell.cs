using FluidSharp.Layouts;
using FluidSharp.State;
using SkiaSharp;
using SkiaSharp.TextBlocks.Enum;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace FluidSharp.Widgets
{
    public class SlidingCell : Widget
    {

        public static TimeSpan DefaultDuration = TimeSpan.FromMilliseconds(150);

        public Widget MainContent;
        public Widget AppearingContent;

        public float AppearingWidth;
        public bool IsAnimating;

        public SlidingCell(Widget mainContent, Widget appearingContent, float appearingWidth, bool isAnimating)
        {
            MainContent = mainContent;
            AppearingContent = appearingContent;
            AppearingWidth = appearingWidth;
            IsAnimating = isAnimating;
        }

        public static Widget MakeWidget(VisualState visualState, object context, Widget child, Func<Widget> near, Func<Widget> far)
        {

            if (child == null) return null;

            var cellstate = visualState.GetOrMake("SlidingCell", () => new SlidingCellState());

            cellstate.CollapseOnDifferentTouchTarget(visualState.TouchTarget);

            if (cellstate.IsContext(context))
            {

                // estimate open ratio
                var (openratio, isanimating) = cellstate.GetOpenRatio(100);

                Widget appearing = null;
                if (openratio > 0 && near != null) appearing = near();
                if (openratio < 0 && far != null) appearing = far();

                if (appearing == null)
                {
                    cellstate.GetOpenRatio(0);
                }
                else
                {

                    try
                    {
                        var fullwidth = appearing.Measure(null, default).Width;

                        // calculate open ratio
                        (openratio, isanimating) = cellstate.GetOpenRatio(fullwidth);

                        child = new SlidingCell(child, appearing, fullwidth * openratio, isanimating);
                    }
                    catch (NullReferenceException)
                    {
                        throw new Exception("sliding cell widgets must have absolute size");
                    }
                }

            }

            return GestureDetector.HorizontalPanDetector(visualState, context, cellstate, child);

        }

        public override SKSize Measure(MeasureCache measureCache, SKSize boundaries)
        {
            return MainContent.Measure(measureCache, boundaries);
        }

        public override SKRect PaintInternal(LayoutSurface layoutsurface, SKRect rect)
        {

            SKRect appearingRect;
            SKRect mainContentRect;

            var showleft = AppearingWidth > 0;
            if (layoutsurface.Device.FlowDirection == FlowDirection.RightToLeft)
                showleft = !showleft;

            var width = AppearingWidth > 0 ? AppearingWidth : -AppearingWidth;
            if (showleft)
            {
                appearingRect = new SKRect(rect.Left, rect.Top, rect.Left + width, 0);
                mainContentRect = new SKRect(rect.Left + width, rect.Top, rect.Right + width, rect.Bottom);
            }
            else
            {
                appearingRect = new SKRect(rect.Right - width, rect.Top, rect.Right, 0);
                mainContentRect = new SKRect(rect.Left - width, rect.Top, rect.Right - width, rect.Bottom);
            }

            layoutsurface.ClipRect(rect);

            var maincontentrect = layoutsurface.Paint(MainContent, mainContentRect);
            appearingRect.Bottom = rect.Top + maincontentrect.Height;
            layoutsurface.Paint(AppearingContent, appearingRect);

            layoutsurface.ResetClip();

            if (IsAnimating)
                layoutsurface.SetHasActiveAnimations();

            return new SKRect(rect.Left, rect.Top, rect.Right, rect.Top + maincontentrect.Height);

        }

    }
}
