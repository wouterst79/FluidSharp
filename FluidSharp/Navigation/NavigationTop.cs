using FluidSharp.State;
using System;
using System.Collections.Generic;
using System.Text;

namespace FluidSharp.Navigation
{
    public class NavigationTop
    {

        private object Context;
        public DateTime Started;

        public NavigationTop() { }

        public NavigationTop(object context)
        {
            Context = context;
            Started = DateTime.UtcNow;
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

