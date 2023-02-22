﻿using FluidSharp.Layouts;
using FluidSharp.State;
using FluidSharp.Widgets.CrossPlatform;
using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Text;

namespace FluidSharp.Widgets
{
    public class Scrollable : Widget
    {

        public ScrollState ScrollState;
        public OverscrollBehavior Overscroll;
        public Widget ChildTree;

        public bool ClipContents = false;
        public Margins ClipMargins;

        public Scrollable(VisualState visualState, object context, PlatformStyle platformStyle, Widget child) :
            this(visualState, context, platformStyle.DefaultOverscrollBehavior, child)
        { }

        public Scrollable(VisualState visualState, object context, OverscrollBehavior overscrollBehavior, Widget child)
        {
            ScrollState = visualState.GetOrMake(context, () => new ScrollState(overscrollBehavior));
            Overscroll = overscrollBehavior;
            ChildTree = GestureDetector.VerticalPanDetector(visualState, ScrollState, child);
        }

        public override SKSize Measure(MeasureCache measureCache, SKSize boundaries)
        {
            return boundaries;
        }

        public override SKRect PaintInternal(LayoutSurface layoutsurface, SKRect rect)
        {

            var childsize = ChildTree.Measure(layoutsurface.MeasureCache, rect.Size);


            var state = ScrollState;
            state.SetRange(rect.Height, childsize.Height);

            var (scroll, overscroll, hasactiveanimations) = state.GetScroll();

            var top = scroll;

            //if(top > 0)
            //{
            //    System.Diagnostics.Debug.WriteLine($"top {top}, over {overscroll}");
            //}

            if (ClipContents)
            {
                var cliprect = rect;
                cliprect = ClipMargins.Grow(cliprect, layoutsurface.Device.FlowDirection);
                layoutsurface.ClipRect(cliprect);
            }

            var childrect = new SKRect(rect.Left, rect.Top + top, rect.Right, rect.Top + top + childsize.Height);
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
