using FluidSharp.Layouts;
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

        public Widget? MeasureWidget;

        public NativeTextboxWidget(object context, string? text, Func<string, Task> settext, Font font, SKColor textcolor, bool hasFocus, Keyboard keyboard, bool sizebytext = false, ReturnTypeInfo? returnTypeInfo = null)
        {
            Context = context;
            Text = text ?? "";
            SetText = settext;
            Font = font;
            TextColor = textcolor;
            HasFocus = hasFocus;
            Keyboard = keyboard;
            ReturnTypeInfo = returnTypeInfo;
            //if (sizebytext)
                MeasureWidget = new Text(font, default, Text + "W");
            //else
              //  ExpandHorizontal = true;
        }

        public override SKSize Measure(MeasureCache measureCache, SKSize boundaries)
        {
            if (MeasureWidget is null) return base.Measure(measureCache, boundaries);
            return MeasureWidget.Measure(measureCache, boundaries);
        }
    }
}
