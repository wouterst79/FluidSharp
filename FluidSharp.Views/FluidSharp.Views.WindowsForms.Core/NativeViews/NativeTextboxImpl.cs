using FluidSharp.Widgets.Native;
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

        public NativeTextboxImpl(Func<Task> requestRedraw)
        {
            RequestRedraw = requestRedraw;
            //            BackColor = Color.Red;
            //          Height = 100;
            //            this.Click += NativeAdViewImpl_Click;
        }

        public void UpdateControl(NativeViewWidget nativeViewWidget)
        {
            var widget = (NativeTextboxWidget)nativeViewWidget;
            if (Text != widget.Text)
            {
                Text = widget.Text;
            }
            SetText = widget.SetText;
        }

        //private void NativeAdViewImpl_Click(object sender, EventArgs e)
        //{
        //    this.Height = Height == 100 ? 150 : 100;
        //    Task.Run(RequestRedraw);
        //}

        protected override void OnTextChanged(EventArgs e)
        {
            base.OnTextChanged(e);
            if (SetText == null)
                System.Diagnostics.Debug.WriteLine("Warning! SetText not set!");
            else
                Task.Run(async () => { await SetText(Text); await RequestRedraw(); });
        }

    }
}
