#if DEBUG
#define PRINTEVENTS
#endif
using FluidSharp.Widgets.Native;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UIKit;

namespace FluidSharp.Views.iOS.NativeViews
{
    public class NativeTextboxImpl : UITextField, INativeViewImpl//, IUITextFieldDelegate
    {

        public object Context;

        private Func<Task> RequestRedraw;
        private Func<string, Task> SetText;
        private bool settingText;

        private Keyboard? Style;

        public NativeTextboxImpl(Func<Task> requestRedraw)
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

        public void UpdateControl(NativeViewWidget nativeViewWidget)
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
            if (Style != widget.Keyboard)
            {
                SetStyle(widget.Keyboard);
            }
            SetText = widget.SetText;
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

        private void SetStyle(Keyboard style)
        {
#if PRINTEVENTS
            Debug.WriteLine($"setting keyboard: {style}");
#endif
            ApplyKeyboard(this, style);
            Style = style;
        }

        // https://github.com/xamarin/Xamarin.Forms/blob/f35ae07a0a8471d255f7a1ebdd51499e10e0a4cb/Xamarin.Forms.Platform.iOS/Extensions/Extensions.cs
        public static void ApplyKeyboard(IUITextInputTraits textInput, Keyboard keyboard)
        {
            textInput.AutocapitalizationType = UITextAutocapitalizationType.None;
            textInput.AutocorrectionType = UITextAutocorrectionType.No;
            textInput.SpellCheckingType = UITextSpellCheckingType.No;
            textInput.KeyboardType = UIKeyboardType.Default;

            if (keyboard == Keyboard.Default)
            {
                textInput.AutocapitalizationType = UITextAutocapitalizationType.Sentences;
                textInput.AutocorrectionType = UITextAutocorrectionType.Default;
                textInput.SpellCheckingType = UITextSpellCheckingType.Default;
            }
            else if (keyboard == Keyboard.Chat)
            {
                textInput.AutocapitalizationType = UITextAutocapitalizationType.Sentences;
                textInput.AutocorrectionType = UITextAutocorrectionType.Yes;
            }
            else if (keyboard == Keyboard.Email)
                textInput.KeyboardType = UIKeyboardType.EmailAddress;
            else if (keyboard == Keyboard.Numeric)
                textInput.KeyboardType = UIKeyboardType.DecimalPad;
            else if (keyboard == Keyboard.Telephone)
                textInput.KeyboardType = UIKeyboardType.PhonePad;
            else if (keyboard == Keyboard.Text)
            {
                textInput.AutocapitalizationType = UITextAutocapitalizationType.Sentences;
                textInput.AutocorrectionType = UITextAutocorrectionType.Yes;
                textInput.SpellCheckingType = UITextSpellCheckingType.Yes;
            }
            else if (keyboard == Keyboard.Url)
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
    }
}
