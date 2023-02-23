﻿using FluidSharp.Animations;
using FluidSharp.Layouts;
using FluidSharp.Paint;
using FluidSharp.State;
using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace FluidSharp.Widgets.Material
{
    public class InkWell : Widget
    {

        public SKColor InkWellColor;
        public Widget ChildTree;
        public Animation Animation;

        public InkWell(ContainerLayout containerLayout, VisualState visualState, object context, SKColor inkWellColor, Func<Task> onTapped, Widget? child)
            : this(containerLayout, visualState, context, inkWellColor, onTapped, null, child) { }

        public InkWell(ContainerLayout containerLayout, VisualState visualState, object context, SKColor inkWellColor, Func<Task> onTapped, Func<Task> onLongTapped, Widget? child)
        {

            InkWellColor = inkWellColor;

            var innerwidget = child ?? new Container(ContainerLayout.Expand);

            if (visualState.TouchTarget.IsContext<TapContext>(context, true))
            {

                var started = visualState.TouchTarget.Started;
                Animation = new Animation(started, Animation.DefaultDuration, .3f, 1);
                innerwidget = new AnimatedWidget(Animation, innerwidget);

            }

            ChildTree = GestureDetector.TapDetector(visualState, context, onTapped, onLongTapped, innerwidget);

            if (containerLayout != ContainerLayout.Wrap)
                ChildTree = new Container(containerLayout, ChildTree);

        }

        public override SKSize Measure(MeasureCache measureCache, SKSize boundaries)
        {
            return ChildTree.Measure(measureCache, boundaries);
        }

        public override SKRect PaintInternal(LayoutSurface layoutsurface, SKRect rect)
        {

            var touchtarget = layoutsurface.VisualState.TouchTarget;
            if (touchtarget != null && Animation != null)
            {

                var canvas = layoutsurface.Canvas;
                if (canvas != null)
                {

                    //var childsize = ChildTree.Measure(layoutsurface.MeasureCache, rect.Size);
                    var childrect = rect;// new SKRect(rect.Left, rect.Top, rect.Right, rect.Top + childsize.Height);

                    layoutsurface.ClipRect(childrect);

                    var result = layoutsurface.Paint(ChildTree, childrect);

                    var sizefactor = Animation.GetValue();

                    var taplocation = touchtarget.LocationOnWidget;
                    var center = new SKPoint(childrect.Left + taplocation.X, childrect.Top + taplocation.Y);

                    var maxw = Math.Max(taplocation.X, childrect.Width - taplocation.X);
                    var maxh = Math.Max(taplocation.Y, childrect.Height - taplocation.Y);

                    var longsize = new SKPoint(maxw, maxh).Length;

                    var radius = longsize * sizefactor;
                    layoutsurface.Canvas.DrawCircle(center, radius, PaintCache.GetBackgroundPaint(InkWellColor));

                    layoutsurface.ResetRectClip();

                    return childrect;


                }

            }

            return layoutsurface.Paint(ChildTree, rect);

        }

    }
}
