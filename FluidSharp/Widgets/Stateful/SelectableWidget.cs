using FluidSharp.Layouts;
using FluidSharp.State;
using FluidSharp.Widgets.CrossPlatform;
using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace FluidSharp.Widgets.Stateful
{
    public class TwoStateWidget : Widget
    {

        private Func<bool> IsSelected;
        private Func<bool, Widget> MakeWidget;

        private Widget? NotSelectedWidget;
        private Widget? SelectedWidget;

        private Widget GetWidget()
        {
            if (IsSelected())
            {
                if (SelectedWidget is null) SelectedWidget = MakeWidget(true);
                return SelectedWidget;
            }
            else
            {
                if (NotSelectedWidget is null) NotSelectedWidget = MakeWidget(false);
                return NotSelectedWidget;
            }
        }

        public TwoStateWidget(Func<bool> isSelected, Func<bool, Widget> makeWidget)
        {
            IsSelected = isSelected;
            MakeWidget = makeWidget;
        }

        public override SKSize Measure(MeasureCache measureCache, SKSize boundaries) => GetWidget().Measure(measureCache, boundaries);
        public override SKRect PaintInternal(LayoutSurface layoutsurface, SKRect rect) => layoutsurface.Paint(GetWidget(), rect);



        //public static Widget Make(PlatformStyle platformStyle, VisualState visualState, object context, Func<Task> onTapped, Widget contents)
        //{
        //    var id = context is string ? context : context.GetType().Name;
        //    return new StatefulButton(visualState, platformStyle.FlatButtonSelectedBackgroundColor, context, onTapped, contents);
        //}

        //public static Widget MakeButton(PlatformStyle platformStyle, VisualState visualState, object context, Func<Task> onTapped, Func<bool> isSelected, Func<bool, Widget> makecontents)
        //{
        //    return Make(platformStyle, visualState, context, onTapped, new SelectableWidget(isSelected, makecontents));
        //}

    }
}
