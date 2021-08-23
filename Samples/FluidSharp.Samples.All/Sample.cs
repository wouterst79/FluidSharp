using FluidSharp.State;
using FluidSharp.Widgets;
using FluidSharp.Widgets.CrossPlatform;
using System;
using System.Collections.Generic;
using System.Text;

namespace FluidSharp.Samples.All
{
    public abstract class Sample
    {

        public abstract string Name { get; }

        public abstract Widget MakeWidget(VisualState visualState);

    }
}
