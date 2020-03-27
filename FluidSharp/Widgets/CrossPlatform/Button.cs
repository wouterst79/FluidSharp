using FluidSharp.State;
using FluidSharp.Widgets.Material;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace FluidSharp.Widgets.CrossPlatform
{
    public class Button
    {

        public static Widget Make(PlatformStyle platformStyle, VisualState visualState, object context, Func<Task> onTapped, bool enabled, Widget child)
        {
            if (!enabled) return child;
            return Make(platformStyle, visualState, context, onTapped, child);
        }

        public static Widget Make(PlatformStyle platformStyle, VisualState visualState, object context, Func<Task> onTapped, Widget child)
        {
            if (child == null) return null;

            if (platformStyle == PlatformStyle.Material)
                return new InkWell(ContainerLayout.FillHorizontal, visualState, context, platformStyle.InkWellColor, onTapped, child);

            if (platformStyle == PlatformStyle.Cupertino)
                return FlatButton.FillHorizontal(visualState, context, platformStyle.FlatButtonSelectedBackgroundColor, onTapped, child);

            if (platformStyle == PlatformStyle.UWP)
                return FlatButton.FillHorizontal(visualState, context, platformStyle.FlatButtonSelectedBackgroundColor, onTapped, child);

            throw new ArgumentOutOfRangeException();
        }

    }
}
