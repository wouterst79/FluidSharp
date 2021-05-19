using System;
using System.Collections.Generic;
using System.Text;

namespace FluidSharp.Widgets.Native
{
    public class ReturnTypeInfo
    {
        public ReturnType ReturnType { get; set; }
        public Action OnReturnPressed { get; set; }

        public ReturnTypeInfo(ReturnType returnType, Action onReturnPressed)
        {
            ReturnType = returnType;
            OnReturnPressed = onReturnPressed ?? throw new ArgumentNullException(nameof(onReturnPressed));
        }
    }
}
