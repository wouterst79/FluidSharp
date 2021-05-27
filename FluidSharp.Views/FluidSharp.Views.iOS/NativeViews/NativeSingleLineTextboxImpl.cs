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
using UIKit;

namespace FluidSharp.Views.iOS.NativeViews
{
    public class NativeSingleLineTextboxImpl : UITextField, INativeViewImpl, INativeTextboxImpl, IUITextFieldDelegate
    {

        public object Context { get; set; }

        private Func<Task> RequestRedraw;
        private Func<string, Task> SetText;
        private bool settingText;

        private Font LastFont;
        private SKColor LastTextColor;
        private Keyboard? Keyboard;

        private ReturnTypeInfo ReturnTypeInfo;

        public NativeSingleLineTextboxImpl(Func<Task> requestRedraw)
        {
            RequestRedraw = requestRedraw;
            BorderStyle = UITextBorderStyle.None;

            Bounds = new CoreGraphics.CGRect(0, 0, 24, 24);
            //Delegate = this;

            EditingChanged += NativeTextboxImpl_EditingChanged;

        }

        private bool _disposed;
        protected override void Dispose(bool disposing)
        {
            if (_disposed) return;
            _disposed = true;
            if (disposing)
            {
                EditingChanged -= NativeTextboxImpl_EditingChanged;
            }
            base.Dispose(disposing);
        }

        private void NativeTextboxImpl_EditingChanged(object sender, EventArgs e)
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
                Debug.WriteLine($"setting text: {widget.Text}");
#endif
                Text = widget.Text;
            }
            if (IsFirstResponder != widget.HasFocus)
            {
                if (widget.HasFocus && CanBecomeFirstResponder)
                {
#if PRINTEVENTS
                    Debug.WriteLine($"setting first responder");
#endif
                    BecomeFirstResponder();
                    //SetNeedsFocusUpdate();
                    //UpdateFocusIfNeeded();
                }
            }
            SetFont(widget.Font.WithTextSize(widget.Font.TextSize * rect.Width / original.Width));
            SetTextColor(widget.TextColor);
            SetKeyboard(widget.Keyboard);
            UpdateReturnType(widget.ReturnTypeInfo);
            SetText = widget.SetText;
        }

        protected void SetFont(Font font)
        {
            if (LastFont == font) return;
            Font = font.ToUIFont();
            LastFont = font;
        }

        protected void SetTextColor(SKColor textColor)
        {
            if (LastTextColor == textColor) return;
            TextColor = textColor.ToUIColor();
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
            Debug.WriteLine($"setting keyboard: {keyboard}");
#endif
            ApplyKeyboard(this, keyboard);
            Keyboard = keyboard;
        }

        void UpdateReturnType(ReturnTypeInfo info)
        {
            if (ReturnTypeInfo?.ReturnType == info?.ReturnType) return;
            ReturnKeyType = (info?.ReturnType ?? ReturnType.Default).ToUIReturnKeyType();
            ReturnTypeInfo = info;
            if (info?.OnReturnPressed is null)
                ShouldReturn = null;
            else
                ShouldReturn = HandleReturnType;
        }

        public virtual bool HandleReturnType(UITextField textField)
        {
            if (ReturnTypeInfo?.OnReturnPressed != null) ReturnTypeInfo.OnReturnPressed();
            return true;
        }



        // https://github.com/xamarin/Xamarin.Forms/blob/f35ae07a0a8471d255f7a1ebdd51499e10e0a4cb/Xamarin.Forms.Platform.iOS/Extensions/Extensions.cs
        public static void ApplyKeyboard(IUITextInputTraits textInput, Keyboard keyboard)
        {

            if (keyboard == FluidSharp.Keyboard.MultiLine) throw new NotImplementedException();

            textInput.AutocapitalizationType = UITextAutocapitalizationType.None;
            textInput.AutocorrectionType = UITextAutocorrectionType.No;
            textInput.SpellCheckingType = UITextSpellCheckingType.No;
            textInput.KeyboardType = UIKeyboardType.Default;

            if (keyboard == FluidSharp.Keyboard.Default)
            {
                textInput.AutocapitalizationType = UITextAutocapitalizationType.Sentences;
                textInput.AutocorrectionType = UITextAutocorrectionType.Default;
                textInput.SpellCheckingType = UITextSpellCheckingType.Default;
            }
            else if (keyboard == FluidSharp.Keyboard.Name)
            {
                textInput.AutocapitalizationType = UITextAutocapitalizationType.Words;
            }
            else if (keyboard == FluidSharp.Keyboard.Chat)
            {
                textInput.AutocapitalizationType = UITextAutocapitalizationType.Sentences;
                textInput.AutocorrectionType = UITextAutocorrectionType.Yes;
            }
            else if (keyboard == FluidSharp.Keyboard.Email)
                textInput.KeyboardType = UIKeyboardType.EmailAddress;
            else if (keyboard == FluidSharp.Keyboard.Numeric)
                textInput.KeyboardType = UIKeyboardType.DecimalPad;
            else if (keyboard == FluidSharp.Keyboard.Telephone)
                textInput.KeyboardType = UIKeyboardType.PhonePad;
            else if (keyboard == FluidSharp.Keyboard.Text)
            {
                textInput.AutocapitalizationType = UITextAutocapitalizationType.Sentences;
                textInput.AutocorrectionType = UITextAutocorrectionType.Yes;
                textInput.SpellCheckingType = UITextSpellCheckingType.Yes;
            }
            else if (keyboard == FluidSharp.Keyboard.Url)
                textInput.KeyboardType = UIKeyboardType.Url;
            //else if (keyboard is CustomKeyboard)
            //{
            //    var custom = (CustomKeyboard)keyboard;

            //    var capitalizedSentenceEnabled = (custom.Flags & KeyboardFlags.CapitalizeSentence) == KeyboardFlags.CapitalizeSentence;
            //    var capitalizedWordsEnabled = (custom.Flags & KeyboardFlags.CapitalizeWord) == KeyboardFlags.CapitalizeWord;
            //    var capitalizedCharacterEnabled = (custom.Flags & KeyboardFlags.CapitalizeCharacter) == KeyboardFlags.CapitalizeCharacter;
            //    var capitalizedNone = (custom.Flags & KeyboardFlags.None) == KeyboardFlags.None;

            //    var spellcheckEnabled = (custom.Flags & KeyboardFlags.Spellcheck) == KeyboardFlags.Spellcheck;
            //    var suggestionsEnabled = (custom.Flags & KeyboardFlags.Suggestions) == KeyboardFlags.Suggestions;


            //    UITextAutocapitalizationType capSettings = UITextAutocapitalizationType.None;

            //    // Sentence being first ensures that the behavior of ALL is backwards compatible
            //    if (capitalizedSentenceEnabled)
            //        capSettings = UITextAutocapitalizationType.Sentences;
            //    else if (capitalizedWordsEnabled)
            //        capSettings = UITextAutocapitalizationType.Words;
            //    else if (capitalizedCharacterEnabled)
            //        capSettings = UITextAutocapitalizationType.AllCharacters;
            //    else if (capitalizedNone)
            //        capSettings = UITextAutocapitalizationType.None;

            //    textInput.AutocapitalizationType = capSettings;
            //    textInput.AutocorrectionType = suggestionsEnabled ? UITextAutocorrectionType.Yes : UITextAutocorrectionType.No;
            //    textInput.SpellCheckingType = spellcheckEnabled ? UITextSpellCheckingType.Yes : UITextSpellCheckingType.No;
            //}
        }

        public void SetVisible(bool visible) => throw new NotImplementedException();
        public void SetBounds(SKRect nativebounds) => throw new NotImplementedException();

    }
}
