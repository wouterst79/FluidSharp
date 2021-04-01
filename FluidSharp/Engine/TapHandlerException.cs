using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace FluidSharp.Engine
{
    public class TapHandlerException : Exception
    {

        private IDictionary Details;
        public override IDictionary Data => Details;

        public TapHandlerException(string message, object context, Exception innerException) : base(message, innerException)
        {
            Details = innerException?.Data ?? new Dictionary<object, object>();
            Details.Add("Context", context);
        }

        public TapHandlerException(string message, object context, Exception innerException, Dictionary<string, string> details) : base(message, innerException)
        {
            Details = details ?? innerException?.Data ?? new Dictionary<object, object>();
            Details.Add("Context", context);
        }

    }
}
