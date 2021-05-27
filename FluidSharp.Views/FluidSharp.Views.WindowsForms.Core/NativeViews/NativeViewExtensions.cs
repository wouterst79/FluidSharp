using SkiaSharp;
using SkiaSharp.TextBlocks;
using UWPFont = System.Drawing.Font;
using UWPColor = System.Drawing.Color;

namespace FluidSharp.Views.WindowsForms.Core.NativeViews
{
    public static class NativeViewExtensions
    {

        public static UWPFont ToUWPFont(this Font font)
        {
            return new UWPFont(font.Name, font.TextSize / 1.5f);
        }

        public static UWPColor ToUWPColor(this SKColor color)
        {
            return UWPColor.FromArgb(color.Alpha, color.Red, color.Green, color.Blue);
        }

    }
}