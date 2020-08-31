using SkiaSharp;
using SkiaSharp.TextBlocks;
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

        public SKColor CheckboxColor = new SKColor(59, 153, 252);
        public SKColor CheckboxStrokeColor = SKColors.Gray;
        public SKColor DisabledColor = SKColors.Gray;
        public SKColor SelectedColor = SKColors.Black.WithAlpha(32);
        public SKColor FieldBorderColor = SKColors.Gray;

        public Widget Separator;
        public Widget OptionalSeparator;
        public Widget InsideListSeparator;

        public bool DeletableCellIsSliding;
        public bool UseFakeBoldText { get => Font.UseFakeBoldText; set => Font.UseFakeBoldText = value; }
        public string DefaultFontName { get => Font.DefaultFontName; set => Font.DefaultFontName = value; }

        public float LineSpacing { get => TextBlock.DefaultLineSpacing; set => TextBlock.DefaultLineSpacing = value; }
        public float FieldCornerRadius = 3;
        public SKSize FieldPadding = new SKSize(5, 3);

        public OverscrollBehavior DefaultOverscrollBehavior = OverscrollBehavior.Stretch;

        public Func<SKImageFilter> DropShadowImageFilterSmall;
        public Func<SKImageFilter> DropShadowImageFilterLarge;



        public PlatformStyle(string name = null)
        {
            Name = name;
            Separator = Rectangle.Horizontal(1, SeparatorGrey);
            OptionalSeparator = null;
            InsideListSeparator = null;// Rectangle.Horizontal(1, SeparatorGrey);
            DropShadowImageFilterSmall = () => SKImageFilter.CreateDropShadow(0, 2.5f, 2, 2, InkWellColor, SKDropShadowImageFilterShadowMode.DrawShadowAndForeground);
            DropShadowImageFilterLarge = () => SKImageFilter.CreateDropShadow(0, 5, 4, 4, InkWellColor, SKDropShadowImageFilterShadowMode.DrawShadowAndForeground);
        }

        public static PlatformStyle Material = new PlatformStyle("Material")
        {
            DefaultOverscrollBehavior = OverscrollBehavior.Invert,
            DefaultFontName = "Roboto",
        };

        private static PlatformStyle cupertino;
        public static PlatformStyle Cupertino
        {
            get
            {
                if (cupertino == null)
                {
                    cupertino = new PlatformStyle("Cupertino");
                    cupertino.OptionalSeparator = cupertino.Separator;
                    cupertino.InsideListSeparator = Rectangle.Horizontal(1, cupertino.SeparatorGrey, new Layouts.Margins(15, 0, 0, 0));
                    cupertino.DeletableCellIsSliding = true;
#if DEBUG
                    cupertino.DefaultFontName = "Roboto";
#endif
                    cupertino.LineSpacing = 1.16f;
                }
                return cupertino;
            }
        }

        public static PlatformStyle UWP = new PlatformStyle("UWP")
        {

        };

    }
}
