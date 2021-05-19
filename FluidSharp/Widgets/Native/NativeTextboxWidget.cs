using SkiaSharp;
using SkiaSharp.TextBlocks;
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

        public Font Font;
        public SKColor TextColor;

        public bool HasFocus;
        public Keyboard Keyboard;

        public ReturnTypeInfo? ReturnTypeInfo;

        public NativeTextboxWidget(object context, string? text, Func<string, Task> settext, Font font, SKColor textcolor, bool hasFocus, Keyboard keyboard, ReturnTypeInfo? returnTypeInfo = null)
        {
            Context = context;
            Text = text ?? "";
            SetText = settext;
            Font = font;
            TextColor = textcolor;
            HasFocus = hasFocus;
            Keyboard = keyboard;
            ReturnTypeInfo = returnTypeInfo;
            ExpandHorizontal = true;
        }

    }
}
