using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UIKit;

namespace FluidSharp.Views.iOS.NativeViews
{
    public class NativeTextboxImpl : UITextField
    {

        public object Context;

        private Func<Task> RequestRedraw;

        public NativeTextboxImpl(Func<Task> requestRedraw)
        {
            RequestRedraw = requestRedraw;
            //            BackColor = Color.Red;
            //          Height = 100;
            //            this.Click += NativeAdViewImpl_Click;
        }

        //private void NativeAdViewImpl_Click(object sender, EventArgs e)
        //{
        //    this.Height = Height == 100 ? 150 : 100;
        //    Task.Run(RequestRedraw);
        //}

    }
}
