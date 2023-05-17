using FluidSharp.State;
using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Text;

namespace FluidSharp.Touch
{
    public class TouchTarget
    {

        private object Context;
        public DateTime Started;
        public SKPoint LocationOnWidget;

        public TouchTarget() { }

        public TouchTarget(object context, SKPoint locationOnWidget)
        {
            Context = context;
            Started = DateTime.UtcNow;
            LocationOnWidget = locationOnWidget;
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
