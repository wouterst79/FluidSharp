using FluidSharp.State;
using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace FluidSharp.Touch
{
    public class EditTarget
    {

        private object? Context;
        public Func<bool, Task<bool>> EndEdit;

        public EditTarget() { Context = null; EndEdit = (validate) => Task.FromResult(true); }
        public EditTarget(object context) { Context = context; EndEdit = (validate) => Task.FromResult(true); }
        public EditTarget(object context, Func<bool, Task<bool>> endEdit)
        {
            Context = context ?? throw new ArgumentNullException(nameof(context));
            EndEdit = endEdit ?? throw new ArgumentNullException(nameof(endEdit));
        }

        public bool IsContext(object context)
        {
            return TypedContext.ContextEqual(Context, context);
        }

        public bool IsContext<T>(object context, bool includederived)
        {
            return TypedContext.ContextEqual<T>(Context, context, includederived);
        }

        public bool IsContext<T>(bool acceptnull)
        {
            return TypedContext.ContextEqual<T>(Context, acceptnull);
        }

    }
}
