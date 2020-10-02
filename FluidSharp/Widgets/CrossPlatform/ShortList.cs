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

        public static Widget Make<T>(PlatformStyle platformStyle, VisualState visualState, object context, IEnumerable<T> items, Func<T, bool> isItemSelected, SKColor selectedColor, Func<T, Task> onItemSelected, Func<T, Widget> makeItemWidget)
        {
            return new Scrollable(visualState, context, platformStyle.DefaultOverscrollBehavior,
                new Column(0)
                {
                    Separator = platformStyle.Separator,
                    Children = items.Select(
                        item => SelectableButton.Make(platformStyle, visualState, item,
                                        makeItemWidget(item), isItemSelected(item), selectedColor, () => onItemSelected(item))
                    ).ToList()
                }
            );
        }

        public static Widget Make<T>(PlatformStyle platformStyle, VisualState visualState, object context, IEnumerable<T> items, Func<T, bool> isItemSelected, SKColor selectedColor, Func<T, Task> onItemSelected, Func<T, Widget> makeItemWidget, Widget? header, Widget? footer)
        {
            var column =
                new Column(0)
                {
                    Separator = platformStyle.Separator,
                    Children = items.Select(
                        item => SelectableButton.Make(platformStyle, visualState, item,
                                        makeItemWidget(item), isItemSelected(item), selectedColor, () => onItemSelected(item))
                    ).ToList()
                };
            if (header != null) column.Children.Insert(0, header);
            if (footer != null) column.Children.Add(footer);
            return new Scrollable(visualState, context, platformStyle.DefaultOverscrollBehavior, column);
        }

    }
}
