using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace FluidSharp.Views.Forms.NativeViews
{
    public class NativeTextboxImpl : Entry
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
