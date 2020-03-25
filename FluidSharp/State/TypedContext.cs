using System;
using System.Collections.Generic;
using System.Text;

namespace FluidSharp.State
{

    public abstract class TypedContext
    {

        public object InnerContext;

        public TypedContext(object innerContext)
        {
            InnerContext = innerContext;
        }

        public virtual bool IsContext(object obj, bool includederived)
        {
            if (InnerContext is ChildContext childContext && !includederived) return false;
            if (InnerContext is TypedContext typedContext)
                return typedContext.IsContext(obj, includederived);
            return Equals(obj);
        }

        public override bool Equals(object obj)
        {
            if (obj is TypedContext context)
            {
                if (obj.GetType() != GetType()) return false;
                return InnerContext.Equals(context.InnerContext);
            }
            return InnerContext.Equals(obj);
        }

        public override int GetHashCode() => 791456222 + InnerContext.GetHashCode();


        public static bool ContextEqual(object Context, object context)
        {
            if (Context == null) return context == null;
            return Context.Equals(context);
        }

        public static bool ContextEqual<T>(object Context, object context, bool includederived)
        {
            if (Context is TapContext tc)
            {
                if (tc.InnerContext is ChildContext)
                {
                    if (tc.Equals(context) && !includederived)
                    {
                        System.Diagnostics.Debug.WriteLine("childcontext");
                    }
                }
            }

            if (Context == null) return context == null;
            if (!(Context is T)) return false;
            if (Context is TypedContext typedContext)
                return typedContext.IsContext(context, includederived);
            return Context.Equals(context);
        }

        public static bool ContextEqual<T>(object Context, bool acceptnull)
        {
            if (Context == null) return acceptnull;
            if (Context is T) return true;
            if (Context is TypedContext typedContext)
                return ContextEqual<T>(typedContext.InnerContext, acceptnull);
            return false;
        }


    }

    public class ChildContext : TypedContext
    {
        public ChildContext(object innerContext, object childcontext) : base(innerContext) { }

        public override bool IsContext(object obj, bool includederived)
        {
            if (!includederived)
                if (obj is TapContext context)
                {
                    if (obj.GetType() != GetType()) return false;
                    return InnerContext.Equals(context.InnerContext);
                }
            return base.IsContext(obj, includederived);
        }
    }

    public class PanContext : TypedContext
    {
        public PanContext(object innerContext) : base(innerContext) { }
    }

    public class TapContext : TypedContext
    {
        public TapContext(object innerContext) : base(innerContext) { }
    }

}
