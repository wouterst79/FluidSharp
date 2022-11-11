using FluidSharp.Widgets.Native;
using Foundation;
using SkiaSharp;
using SkiaSharp.TextBlocks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UIKit;

namespace FluidSharp.Views.iOS.NativeViews
{
    public static class NativeViewExtensions
    {

        public static UIFont ToUIFont(this Font font)
        {
            UIFont result;
            if (font.FontStyle.Weight == 600)//SKFontStyleWeight.SemiBold)
            {
                result = UIFont.FromName(font.Name + " Semibold", font.TextSize);
                if (result != null) return result;
            }
            result = UIFont.FromName(font.Name, font.TextSize);
            return result;
        }

        public static UIColor ToUIColor(this SKColor color)
        {
            return new UIColor(color.Red / 255f, color.Green / 255f, color.Blue / 255f, color.Alpha / 255f);
        }

        internal static UIReturnKeyType ToUIReturnKeyType(this ReturnType returnType)
        {
            switch (returnType)
            {
                case ReturnType.Go:
                    return UIReturnKeyType.Go;
                case ReturnType.Next:
                    return UIReturnKeyType.Next;
                case ReturnType.Send:
                    return UIReturnKeyType.Send;
                case ReturnType.Search:
                    return UIReturnKeyType.Search;
                case ReturnType.Done:
                    return UIReturnKeyType.Done;
                case ReturnType.Default:
                    return UIReturnKeyType.Default;
                default:
                    throw new System.NotImplementedException($"ReturnType {returnType} not supported");
            }
        }
    }
}