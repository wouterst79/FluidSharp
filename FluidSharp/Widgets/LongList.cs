﻿using FluidSharp.State;
using FluidSharp.Widgets.CrossPlatform;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace FluidSharp.Widgets
{

    /// <summary>
    /// A long list widget is similar to 
    /// </summary>
    public class LongList
    {

        public static Widget Make<T>(PlatformStyle platformStyle, VisualState visualState, object context, IList<T> items, float itemHeight, Func<T, Task> onItemSelected, Func<T, Widget> makeItemWidget, bool clipcontents)
        {

            Func<T, Widget> makebutton = (item) => CrossButton.Make(platformStyle, visualState, item, () => onItemSelected(item), makeItemWidget(item));

            return new Scrollable(visualState, context, platformStyle.DefaultOverscrollBehavior,
                new LongListContents<T>(items, itemHeight, onItemSelected, makebutton)
            )
            { ClipContents = clipcontents } ;

        }

        //public static Widget Make<T>(VisualState visualState, IEnumerable<T> items, Func<T, float> getItemHeight, Func<T, Task> onItemSelected, Func<T, Widget> makeItemWidget)
        //{
        //    public static Widget Make<T>(PlatformStyle platformStyle, VisualState visualState, IEnumerable<T> items, Func<T, bool> isItemSelected, SKColor selectedColor, Func<T, Task> onItemSelected, Func<T, Widget> makeItemWidget)
        //    {
        //        return new Scrollable(visualState, items, platformStyle.DefaultOverscrollBehavior,
        //            new Column()
        //            {
        //                Children = items.Select(
        //                    item => MakeSelectableButton(platformStyle, visualState, item,
        //                                    makeItemWidget(item), isItemSelected(item), selectedColor, () => onItemSelected(item))
        //                ).ToList()
        //            }
        //        );
        //    }

        //}
    }
}
