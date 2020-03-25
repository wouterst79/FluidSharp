using FluidSharp.State;
using FluidSharp.Widgets;
using System;
using System.Collections.Generic;
using System.Text;

namespace FluidSharp
{

    /// <summary>
    /// Implementations of this interface produce Widgets to be rendered by FluidSharp
    /// </summary>
    public interface IWidgetSource
    {
        Widget MakeWidget(VisualState visualState);
    }
}
