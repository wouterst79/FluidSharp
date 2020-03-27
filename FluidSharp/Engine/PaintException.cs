using System;
using System.Collections.Generic;
using System.Text;

namespace FluidSharp.Engine
{
    public class PaintException : Exception
    {
        public PaintException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}
