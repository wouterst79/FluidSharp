﻿using FluidSharp.State;
using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace FluidSharp.Widgets.CrossPlatform
{
    public class SelectableButtonWidget
    {

        public static Widget Make(PlatformStyle platformStyle, VisualState visualState, object context, Widget contents, bool ischecked, SKColor selectedcolor, Func<Task> onTapped, ContainerLayout containerLayout = ContainerLayout.FillHorizontal)
        {

            var istouchtarget = visualState.TouchTarget.IsContext<TapContext>(context, false);
            var hasbackground = istouchtarget || ischecked;
            var backgroundcolor = hasbackground ?
                                  (istouchtarget ? platformStyle.FlatButtonSelectedBackgroundColor : selectedcolor)
                                  : default;

            var innerwidget = new Container(containerLayout)
            {
                MinimumSize = new SKSize(0, 10),
                Children =
                {
                    hasbackground ? Rectangle.Fill(backgroundcolor) : null,
                    Align.Center(contents)
                }
            };

            return GestureDetector.TapDetector(visualState, context, onTapped, null, innerwidget);
        }


    }
}
