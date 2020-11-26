using FluidSharp.Widgets.Native;
using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FluidSharp.Views.WindowsForms.NativeViews
{
    public class NativeTextboxImpl : TextBox, INativeViewImpl
    {

        public object Context;

        private Func<Task> RequestRedraw;
        private Func<string, Task> SetText;
        private bool settingText;

        public NativeTextboxImpl(Func<Task> requestRedraw)
        {
            RequestRedraw = requestRedraw;
            BorderStyle = BorderStyle.None;
        }

        public void UpdateControl(NativeViewWidget nativeViewWidget)
        {
            var widget = (NativeTextboxWidget)nativeViewWidget;
            SetText = null;
            if (!settingText && Text != widget.Text)
            {
                Text = widget.Text;
            }
            if (Focused != widget.HasFocus)
            {
                if (widget.HasFocus && CanFocus)
                    Focus();
            }
            SetText = widget.SetText;
        }

        protected override void OnTextChanged(EventArgs e)
        {
            base.OnTextChanged(e);
            if (SetText != null)
            {
                Task.Run(async () =>
                {
                    try
                    {
                        settingText = true;
                        await SetText(Text);
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
