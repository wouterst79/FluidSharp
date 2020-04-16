using FluidSharp.State;
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

        public static Widget Make<T>(PlatformStyle platformStyle, VisualState visualState, IList<T> items, float itemHeight, Func<T, Task> onItemSelected, Func<T, Widget> makeItemWidget)
        {

            Func<T, Widget> makebutton = (item) => Button.Make(platformStyle, visualState, item, () => onItemSelected(item), makeItemWidget(item));

            return new Scrollable(visualState, items, platformStyle.DefaultOverscrollBehavior,
                new LongListContents<T>(items, itemHeight, onItemSelected, makebutton)
            ); ;

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
