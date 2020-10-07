using FluidSharp.State;
using FluidSharp.Widgets;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace FluidSharp.Navigation
{
    public interface IPageTransition
    {

        public Widget MakeWidget(VisualState visualState, IWidgetSource from, IWidgetSource to, Func<Task> dismiss);

        public Task Start();
        public Task Reverse();

    }
}
