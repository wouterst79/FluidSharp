using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace FluidSharp.Engine
{
    public class MeasureException : Exception
    {

        private Dictionary<string, string> Details;
        public override IDictionary Data => Details;

        public MeasureException(string message, Exception innerException, Dictionary<string, string> details) : base(message, innerException)
        {   
            Details = details;
        }
    }
}
