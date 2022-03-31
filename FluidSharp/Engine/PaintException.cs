using FluidSharp.Widgets;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace FluidSharp.Engine
{
    public class PaintException : Exception
    {


        public Widget Widget { get; set; }

        private IDictionary? Details;
        public override IDictionary Data => Details ?? new Dictionary<string, string>();

        public PaintException(string message, Widget widget, Exception innerException) : base(message, innerException)
        {
            Widget = widget;
            Details = innerException?.Data;
        }

        public PaintException(string message, Widget widget, Exception innerException, Dictionary<string, string>? details) : base(message, innerException)
        {
            Widget = widget;
            Details = details ?? innerException?.Data;
        }

    }
}
