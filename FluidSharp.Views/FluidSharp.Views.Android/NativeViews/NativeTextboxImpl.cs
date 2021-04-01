#if DEBUG
#define PRINTEVENTS
#endif
using FluidSharp.Widgets.Native;
using SkiaSharp;
using SkiaSharp.TextBlocks;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Android.Views;
using Android.Widget;
using Android.Content;
using Android.Util;
using Android.Content.Res;

namespace FluidSharp.Views.Android.NativeViews
{
    public class NativeTextboxImpl : EditText, INativeViewImpl//, IUITextFieldDelegate
    {

        public new object Context;

        private Func<Task> RequestRedraw;
        private new Func<string, Task> SetText;
        private bool settingText;

        private Font LastFont;
        private SKColor LastTextColor;
        private Keyboard? Keyboard;


        public NativeTextboxImpl(Context context, Func<Task> requestRedraw) : base(context)
        {

            RequestRedraw = requestRedraw;

            //Background = null;

            //Delegate = this;

            TextChanged += NativeTextboxImpl_TextChanged;

        }

        private bool _disposed;
        protected override void Dispose(bool disposing)
        {
            if (_disposed) return;
            _disposed = true;
            if (disposing)
            {
                TextChanged -= NativeTextboxImpl_TextChanged;
            }
            base.Dispose(disposing);
        }

        private void NativeTextboxImpl_TextChanged(object sender, EventArgs e)
        {
            OnTextChanged();
        }

        public void UpdateControl(NativeViewWidget nativeViewWidget, SKRect rect, SKRect original)
        {
            var widget = (NativeTextboxWidget)nativeViewWidget;
            SetText = null;
            if (!settingText && Text != widget.Text)
            {
#if PRINTEVENTS
                System.Diagnostics.Debug.WriteLine($"setting text: {widget.Text}");
#endif
                Text = widget.Text;
            }
            if (IsFocused != widget.HasFocus)
            {
                if (widget.HasFocus && Focusable)
                {
#if PRINTEVENTS
                    System.Diagnostics.Debug.WriteLine($"setting focus");
#endif
                    this.RequestFocus();
                    //BecomeFirstResponder();
                    //SetNeedsFocusUpdate();
                    //UpdateFocusIfNeeded();
                }
            }
            SetFont(widget.Font.WithTextSize(widget.Font.TextSize * rect.Width / original.Width));
            SetTextColor(widget.TextColor);
            SetKeyboard(widget.Keyboard);
            SetText = widget.SetText;
        }

        protected void SetFont(Font font)
        {
            if (LastFont == font) return;
            Typeface = font.ToTypeface();
            SetTextSize(ComplexUnitType.Sp, (float)font.TextSize);
            LastFont = font;
        }

        protected void SetTextColor(SKColor textColor)
        {
            if (LastTextColor == textColor) return;

            var acolor = textColor.ToAndroid().ToArgb();
            int[][] s_colorStates = { new[] { global::Android.Resource.Attribute.StateEnabled }, new[] { -global::Android.Resource.Attribute.StateEnabled } };
            SetTextColor(new ColorStateList(s_colorStates, new[] { acolor, acolor }));

            LastTextColor = textColor;

        }

        protected void OnTextChanged()
        {
            if (SetText != null)
            {
                var text = Text;
                Task.Run(async () =>
                {
                    try
                    {
                        settingText = true;
                        await SetText(text);
                        await RequestRedraw();
                    }
                    finally
                    {
                        settingText = false;
                    }
                });
            }
        }



        private void SetKeyboard(Keyboard keyboard)
        {

            if (Keyboard == keyboard) return;

#if PRINTEVENTS
            System.Diagnostics.Debug.WriteLine($"setting keyboard: {keyboard}");
#endif
            InputType = keyboard.ToInputType();
            Keyboard = keyboard;
        }


    }
}
