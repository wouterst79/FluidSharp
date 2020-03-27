using SkiaSharp;
using SkiaSharp.Views.Forms;
using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;

namespace FluidSharp.Views.Forms
{
    public static class XamarinExtensions
    {

        public static SKColor SKColor(this ResourceDictionary resourceDictionary, string ResourceName)
        {
            var c = resourceDictionary[ResourceName];
            Color color;
            if (c is OnPlatform<Color> opc)
                color = opc;
            else
                color = (Color)c;
            return color.ToSKColor();
        }

        public static SKColor ColorFromResources(string ResourceName) => Application.Current.Resources.SKColor(ResourceName);

    }
}
