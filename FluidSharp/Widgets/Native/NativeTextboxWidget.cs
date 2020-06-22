using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace FluidSharp.Widgets.Native
{
    public class NativeTextboxWidget : NativeViewWidget
    {

        public object Context;

        public string Text;
        public Func<string, Task> SetText;

        public NativeTextboxWidget(object context, string text, Func<string, Task> settext)
        {
            Context = context;
            Text = text ?? "";
            SetText = settext;
        }

    }
}
