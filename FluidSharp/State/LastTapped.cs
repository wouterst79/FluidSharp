using System;
using System.Collections.Generic;
using System.Text;

namespace FluidSharp.State
{
    public class LastTapped
    {

        public object Context;
        public DateTime Tapped;

        private LastTapped(object context)
        {
            Context = context;
            Tapped = DateTime.UtcNow;
        }

        public static void SetContext(VisualState visualState, object context)
        {
            if (!(context is TapContext)) 
                throw new ArgumentOutOfRangeException("LastTapped context must be of type TapContext");
            visualState["lasttapped"] = new LastTapped(context);
        }

        public static DateTime? ForContext(VisualState visualState, object context)
        {
            var lasttapped = visualState.GetOrDefault<LastTapped>("lasttapped");
            //System.Diagnostics.Debug.WriteLine($"{lasttapped?.Context} - {context}");
            if (lasttapped != null && TypedContext.ContextEqual<TapContext>(lasttapped.Context, context, false))
                return lasttapped.Tapped;
            else
                return null;
        }

    }
}
