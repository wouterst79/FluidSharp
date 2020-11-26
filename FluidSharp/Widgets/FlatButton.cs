using FluidSharp.Layouts;
using FluidSharp.State;
using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace FluidSharp.Widgets
{
    public class FlatButton
    {

        public static Widget Fill(VisualState visualState, object context, SKColor selectedBackgroundColor, Func<Task> onTapped, Widget child)
        {

            var innerwidget = child;

            if (visualState.TouchTarget.IsContext<TapContext>(context, false))
                innerwidget = new Container(ContainerLayout.Fill)
                {
                    Children =
                    {
                        Rectangle.Fill(selectedBackgroundColor),
                        innerwidget
                    }
                };

            return GestureDetector.TapDetector(visualState, context, onTapped, null, innerwidget);

        }

        public static Widget FillHorizontal(VisualState visualState, object context, SKColor selectedBackgroundColor, Func<Task> onTapped, Widget child, Func<Task>? onLongTapped = null)
        {

            var innerwidget = child;

            if (visualState.TouchTarget.IsContext<TapContext>(context, false))
                innerwidget = new Container(ContainerLayout.FillHorizontal)
                {
                    Children =
                    {
                        Rectangle.Fill(selectedBackgroundColor),
                        innerwidget
                    }
                };

            return GestureDetector.TapDetector(visualState, context, onTapped, onLongTapped, innerwidget);

        }

        public static Widget Wrap(VisualState visualState, object context, SKColor selectedBackgroundColor, Func<Task> onTapped, Widget child)
        {

            var innerwidget = child;

            if (visualState.TouchTarget.IsContext<TapContext>(context, false))
                innerwidget = new Container(ContainerLayout.Wrap)
                {
                    Children =
                    {
                        Rectangle.Fill(selectedBackgroundColor),
                        innerwidget
                    }
                };

            return GestureDetector.TapDetector(visualState, context, onTapped, null, innerwidget);

        }


    }
}
