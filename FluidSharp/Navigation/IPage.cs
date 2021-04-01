using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace FluidSharp.Navigation
{
    public interface IPage : IWidgetSource
    {

        public bool CanSlideBack { get; }

        public IPageTransition? GetPageTransition(bool startingstate, Func<bool, Task> onTransitionCompleted);

        public Func<Task>? PrepareForResurface();

    }
}
