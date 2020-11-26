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

    }
}
