using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using FluidSharp.State;
using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FluidSharp.Views.Android
{
    public abstract class AndroidFluidWidgetView : RelativeLayout
    {

        public abstract SKSize PlatformScale { get; }
        public abstract VisualState VisualState { get; }

        protected AndroidFluidWidgetView(Context context) : base(context)
        {
        }

        public void AddOnMainThread(View childview)
        {

            ((Activity)Context).RunOnUiThread(() =>
            {
                if (childview.Parent == this) return;
                if (childview.Parent != null)
                    ((ViewGroup)childview.Parent).RemoveView(childview);
                AddView(childview);
            });

        }

    }
}