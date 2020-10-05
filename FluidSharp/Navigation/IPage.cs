using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace FluidSharp.Navigation
{
    public interface IPage : IWidgetSource
    {

        public bool CanSlideBack { get; }

        public IPageTransition? GetPageTransition(Func<bool, Task> onTransitionCompleted);

    }
}
