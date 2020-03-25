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

        public static Widget Make<T>(PlatformStyle platformStyle, VisualState visualState, IEnumerable<T> items, Func<T, bool> isItemSelected, SKColor selectedColor, Func<T, Task> onItemSelected, Func<T,Widget> makeItemWidget)
        {
            return new Scrollable(visualState, items, platformStyle.DefaultOverscrollBehavior,
                new Column()
                {
                    Children = items.Select(
                        item => MakeSelectableButton(platformStyle, visualState, item, 
                                        makeItemWidget(item), isItemSelected(item), selectedColor, () => onItemSelected(item))
                    ).ToList()
                }
            );
        }

        private static Widget MakeSelectableButton(PlatformStyle platformStyle, VisualState visualState, object context, Widget contents, bool ischecked, SKColor selectedcolor, Func<Task> onTapped)
        {

            Widget innerwidget = Align.Center(contents);

            if (visualState.TouchTarget.IsContext<TapContext>(context, false))
            {
                innerwidget = new Container(ContainerLayout.Fill)
                {
                    Children =
                    {
                        Rectangle.Fill(platformStyle.FlatButtonSelectedBackgroundColor),
                        innerwidget
                    }
                };
            }
            else if (ischecked)
            {
                innerwidget = new Container(ContainerLayout.Fill)
                {
                    Children =
                    {
                        Rectangle.Fill(selectedcolor),
                        innerwidget
                    }
                };
            }

            return GestureDetector.TapDetector(visualState, context, onTapped, null, innerwidget);
        }

    }
}
