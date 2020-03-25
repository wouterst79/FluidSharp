using FluidSharp.Interop;
using FluidSharp.State;
using FluidSharp.Widgets;
using System;
using System.Collections.Generic;
using System.Text;

namespace FluidSharp
{

    public interface IFluidWidgetView
    {

        bool AutoSizeHeight { get;  }
        void SetHeight(float Height);

        INativeViewManager GetNativeViewManager();

        Widget MakeWidget(VisualState visualState);

    }

}
