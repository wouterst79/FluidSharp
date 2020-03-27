using System;
using System.Collections.Generic;
using System.Text;

namespace FluidSharp.Engine
{
    public class EngineException : Exception
    {
        public EngineException(string message) : base(message)
        {
        }
        public EngineException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}
