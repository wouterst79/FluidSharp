#if DEBUG
#define PRINTEVENTS
#endif

namespace FluidSharp.Views.iOS.NativeViews
{
    public interface INativeTextboxImpl
    {
        object Context { get; set; }
    }
}
