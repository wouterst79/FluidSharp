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
    public class SlideOverContainer : Widget
    {

        public enum SlideOverDirection
        {
            NearToFar,
            FarToNear,
            TopToBottom,
            BottomToTop
        }


        public static TimeSpan DefaultDuration = TimeSpan.FromMilliseconds(250);

        public Widget MainContent;
        public Widget AppearingContent;

        public SlideOverDirection Direction;
        public bool PushMainContent;
        public float MaxOpen;
        public float OpenRatio;
        public bool IsAnimating;

        public SlideOverContainer(Widget mainContent, Widget appearingContent, SlideOverDirection direction, bool pushMainContent, float maxopen, float openratio, bool isAnimating)
        {
            MainContent = mainContent;
            AppearingContent = appearingContent;
            Direction = direction;
            PushMainContent = pushMainContent;
            MaxOpen = maxopen;
            OpenRatio = openratio;
            IsAnimating = isAnimating;
        }

        public static Widget MakeWidget(VisualState visualState, object context, SlideOverDirection direction, bool pushMainContent, float maxopen, Func<Widget> makechild, Func<Widget> makeappearing)
        {

            var (openratio, isanimating) = SlideOverState.GetRatio(visualState, context);

            if (openratio == 0)
                return makechild();

            if (openratio == 1 && maxopen >= 1)
                return makeappearing();



            var container = new SlideOverContainer(makechild(), makeappearing(), direction, pushMainContent, maxopen, openratio, isanimating);

            if (direction == SlideOverDirection.NearToFar || direction == SlideOverDirection.FarToNear)
                return new GestureDetector.HorizontalPanGestureDetector(visualState, context, false, null, velocity => SlideOverState.GetState(visualState).EndPan(velocity, visualState), container);
            else
                return new GestureDetector.VerticalPanGestureDetector(visualState, context, false, null, velocity => SlideOverState.GetState(visualState).EndPan(velocity, visualState), container);

        }

        public override SKSize Measure(MeasureCache measureCache, SKSize boundaries)
        {
            return MainContent.Measure(measureCache, boundaries);
        }

        public override SKRect PaintInternal(LayoutSurface layoutsurface, SKRect rect)
        {

            SKRect mainContentClipRect = rect;
            SKRect mainContentDrawRect = rect;
            SKRect appearingClipRect;
            SKRect appearingDrawRect;

            if (Direction == SlideOverDirection.NearToFar || Direction == SlideOverDirection.FarToNear)
            {

                var lefttoright = Direction == SlideOverDirection.NearToFar;
                if (layoutsurface.Device.FlowDirection == FlowDirection.RightToLeft)
                    lefttoright = !lefttoright;

                var targetWidth = rect.Width * MaxOpen;
                var appearingWidth = targetWidth * OpenRatio;

                if (lefttoright)
                {
                    mainContentClipRect.Left = rect.Left + appearingWidth;
                    if (PushMainContent) { mainContentDrawRect.Left += appearingWidth; mainContentDrawRect.Right += appearingWidth; }
                    appearingClipRect = new SKRect(rect.Left, rect.Top, rect.Left + appearingWidth, rect.Bottom);
                    appearingDrawRect = new SKRect(rect.Left, rect.Top, rect.Left + targetWidth, rect.Bottom);
                }
                else // right to left
                {
                    mainContentClipRect.Right = rect.Right - appearingWidth;
                    if (PushMainContent) { mainContentDrawRect.Left -= appearingWidth; mainContentDrawRect.Right -= appearingWidth; }
                    appearingClipRect = new SKRect(rect.Right - appearingWidth, rect.Top, rect.Right, rect.Bottom);
                    appearingDrawRect = new SKRect(rect.Right - appearingWidth, rect.Top, rect.Right - appearingWidth + targetWidth, rect.Bottom);
                }

            }
            else
            {

                var targetheight = rect.Height * MaxOpen;
                var appearingHeight = targetheight * OpenRatio;

                if (Direction == SlideOverDirection.TopToBottom)
                {
                    mainContentClipRect.Top = rect.Top + appearingHeight;
                    if (PushMainContent) { mainContentDrawRect.Top += appearingHeight; mainContentDrawRect.Bottom += appearingHeight; }
                    appearingClipRect = new SKRect(rect.Left, rect.Top, rect.Right, rect.Top + appearingHeight);
                    appearingDrawRect = new SKRect(rect.Left, rect.Top, rect.Right, rect.Top + targetheight);
                }
                else // BottomToTop
                {
                    mainContentClipRect.Bottom = rect.Bottom - appearingHeight;
                    if (PushMainContent) { mainContentDrawRect.Top -= appearingHeight; mainContentDrawRect.Bottom -= appearingHeight; }
                    appearingClipRect = new SKRect(rect.Left, rect.Bottom - appearingHeight, rect.Right, rect.Bottom);
                    appearingDrawRect = new SKRect(rect.Left, rect.Bottom - appearingHeight, rect.Right, rect.Bottom - appearingHeight + targetheight);
                }

            }

            layoutsurface.ClipRect(mainContentClipRect);
            layoutsurface.Paint(MainContent, mainContentDrawRect);
            layoutsurface.ResetRectClip();

            layoutsurface.ClipRect(appearingClipRect);
            layoutsurface.Paint(AppearingContent, appearingDrawRect);
            layoutsurface.ResetRectClip();

            if (IsAnimating)
                layoutsurface.SetHasActiveAnimations();

            return new SKRect(rect.Left, rect.Top, rect.Right, rect.Bottom);

        }

    }
}
