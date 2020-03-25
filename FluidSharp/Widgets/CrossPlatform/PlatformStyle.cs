using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Text;
using static FluidSharp.Widgets.Scrollable;

namespace FluidSharp.Widgets.CrossPlatform
{
    public class PlatformStyle
    {


        public readonly string Name;

        public SKColor SeparatorGrey = SKColor.Parse("#E4E3E5");// SKColors.LightGray;

        public SKColor InkWellColor = SKColors.Black.WithAlpha(64);
        public SKColor FlatButtonSelectedBackgroundColor = SKColors.Black.WithAlpha(32);

        public Widget Separator;
        public Widget InsideListSeparator;

        public bool DeletableCellIsSliding;

        public OverscrollBehavior DefaultOverscrollBehavior = OverscrollBehavior.Stretch;



        public PlatformStyle(string name = null)
        {
            Name = name;
            Separator = Rectangle.Horizontal(1, SeparatorGrey);
            InsideListSeparator = Rectangle.Horizontal(1, SeparatorGrey);
        }

        public static PlatformStyle Material = new PlatformStyle("Material")
        {
            DefaultOverscrollBehavior = OverscrollBehavior.Invert
        };

        private static PlatformStyle cupertino;
        public static PlatformStyle Cupertino
        {
            get
            {
                if (cupertino == null)
                {
                    cupertino = new PlatformStyle("Cupertino");
                    cupertino.InsideListSeparator = Rectangle.Horizontal(1, cupertino.SeparatorGrey, new Layouts.Margins(15, 0, 0, 0));
                    cupertino.DeletableCellIsSliding = true;
                }
                return cupertino;
            }
        }

        public static PlatformStyle UWP = new PlatformStyle("UWP")
        {

        };

    }
}
