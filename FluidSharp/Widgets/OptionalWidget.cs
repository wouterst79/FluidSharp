using FluidSharp.Layouts;
using FluidSharp.State;
using FluidSharp.Widgets.CrossPlatform;
using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace FluidSharp.Widgets
{
    public class OptionalWidget : Widget
    {

        private Func<bool> IsVisible;

        private Widget Contents;

        public OptionalWidget(Func<bool> isVisible, Widget contents)
        {
            IsVisible = isVisible ?? throw new ArgumentNullException(nameof(isVisible));
            Contents = contents ?? throw new ArgumentNullException(nameof(contents));
        }

        public override SKSize Measure(MeasureCache measureCache, SKSize boundaries)
        {
            return IsVisible() ? Contents.Measure(measureCache, boundaries) : new SKSize();
        }

        public override SKRect PaintInternal(LayoutSurface layoutsurface, SKRect rect)
        {
            return IsVisible() ? layoutsurface.Paint(Contents, rect) : rect.WithHeight(0);
        }



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
