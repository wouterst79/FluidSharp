using FluidSharp.Views.WindowsForms.Core.NativeViews;
using FluidSharp.Widgets.Native;
using SkiaSharp;
using SkiaSharp.TextBlocks;
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

        private Font LastFont;
        private SKColor LastTextColor;

        public NativeTextboxImpl(Func<Task> requestRedraw)
        {
            RequestRedraw = requestRedraw;
            BorderStyle = BorderStyle.None;
        }

        public void UpdateControl(NativeViewWidget nativeViewWidget, SKRect rect, SKRect original)
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
            SetFont(widget.Font.WithTextSize(widget.Font.TextSize * rect.Width / original.Width));
            SetTextColor(widget.TextColor);
            SetText = widget.SetText;
        }

        protected void SetFont(Font font)
        {
            if (LastFont == font) return;
            Font = font.ToUWPFont();
            LastFont = font;
        }

        protected void SetTextColor(SKColor textColor)
        {
            if (LastTextColor == textColor) return;
            ForeColor = textColor.ToUWPColor();
            LastTextColor = textColor;
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
