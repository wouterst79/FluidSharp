using FluidSharp.State;
using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FluidSharp.Widgets.CrossPlatform
{
    public class ShortList
    {

        public static Widget Make<T>(PlatformStyle platformStyle, VisualState visualState, IEnumerable<T> items, Func<T, bool> isItemSelected, SKColor selectedColor, Func<T, Task> onItemSelected, Func<T, Widget> makeItemWidget)
        {
            return new Scrollable(visualState, items, platformStyle.DefaultOverscrollBehavior,
                new Column()
                {
                    Separator = platformStyle.Separator,
                    Children = items.Select(
                        item => SelectableButton.Make(platformStyle, visualState, item,
                                        makeItemWidget(item), isItemSelected(item), selectedColor, () => onItemSelected(item))
                    ).ToList()
                }
            );
        }

    }
}
