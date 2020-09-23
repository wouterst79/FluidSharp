using FluidSharp.Layouts;
using FluidSharp.State;
using FluidSharp.Widgets.CrossPlatform;
using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Text;

namespace FluidSharp.Widgets
{
    public class Slidable : Widget
    {

        public ScrollState ScrollState;
        public OverscrollBehavior Overscroll;
        public Widget ChildTree;

        public bool ClipContents = false;
        public Margins ClipMargins;

        public Slidable(VisualState visualState, object context, PlatformStyle platformStyle, Widget child) :
            this(visualState, context, platformStyle.DefaultOverscrollBehavior, child)
        { }

        public Slidable(VisualState visualState, object context, OverscrollBehavior overscrollBehavior, Widget child)
        {
            ScrollState = visualState.GetOrMake(context, () => new ScrollState(overscrollBehavior));
            Overscroll = overscrollBehavior;
            ChildTree = GestureDetector.HorizontalPanDetector(visualState, ScrollState, child);
        }


        //public static Widget WithIndicator(VisualState visualState, object context, PlatformStyle platformStyle, Widget child)
        //{

        //    return new Container(ContainerLayout.Wrap,
        //            new Slidable(visualState, context, platformStyle, child),
        //            child
        //        );

        //}


        public override SKSize Measure(MeasureCache measureCache, SKSize boundaries)
        {
            var childsize = ChildTree.Measure(measureCache, boundaries);
            return new SKSize(boundaries.Width, childsize.Height);
        }

        public override SKRect PaintInternal(LayoutSurface layoutsurface, SKRect rect)
        {

            var childsize = ChildTree.Measure(layoutsurface.MeasureCache, rect.Size);


            var state = ScrollState;
            state.SetRange(rect.Width, childsize.Width);

            var (scroll, overscroll, hasactiveanimations) = state.GetScroll();

            var height = childsize.Height;

            //System.Diagnostics.Debug.WriteLine($"left {left}, over {overscroll} ({rect.Width} - {childsize.Width})");

            rect = rect.WithHeight(height);
            if (ClipContents)
            {
                var cliprect = ClipMargins.Grow(rect, layoutsurface.Device.FlowDirection);
                layoutsurface.ClipRect(cliprect);
            }

            var childrect =
                layoutsurface.Device.FlowDirection == SkiaSharp.TextBlocks.Enum.FlowDirection.LeftToRight ?
                new SKRect(rect.Left + scroll, rect.Top, rect.Left + scroll + childsize.Width, rect.Top + height)
             : new SKRect(rect.Right - scroll - childsize.Width, rect.Top, rect.Right - scroll, rect.Top + height);

            layoutsurface.Paint(ChildTree, childrect);

            if (ClipContents)
                layoutsurface.ResetRectClip();

            //layoutsurface.DebugRect(childrect, SKColors.Purple);

            if (hasactiveanimations)
                layoutsurface.SetHasActiveAnimations();

            return rect;

        }

    }
}
