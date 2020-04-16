using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace FluidSharp.Engine
{
    public class MakeWidgetException : Exception
    {

        private IDictionary Details;
        public override IDictionary Data => Details;

        public MakeWidgetException(string message, Exception innerException) : base(message, innerException)
        {
            Details = innerException?.Data;
        }

        public MakeWidgetException(string message, Exception innerException, Dictionary<string, string> details) : base(message, innerException)
        {
            Details = details ?? innerException?.Data;
        }

    }
}
