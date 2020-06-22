using FluidSharp.Layouts;
using FluidSharp.Widgets.CrossPlatform;
using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Text;

namespace FluidSharp.Widgets
{
    public class Field : Widget
    {

        public bool Material;
        public SKColor BorderColor;
        public float CornerRadius;
        public SKSize Padding;

        public Widget Child;

        public Field(PlatformStyle platformStyle, Widget child) : this(platformStyle == PlatformStyle.Material, platformStyle.FieldBorderColor, platformStyle.FieldCornerRadius, platformStyle.FieldPadding, child)
        {
        }

        public Field(bool material, SKColor borderColor, float cornerRadius, SKSize padding, Widget child)
        {
            Material = material;
            BorderColor = borderColor;
            CornerRadius = cornerRadius;
            Padding = padding;
            Child = child;
        }

        public override SKSize Measure(MeasureCache measureCache, SKSize boundaries)
        {
            var doublepadding = Padding + Padding;

            if (Child == null) return doublepadding;

            var available = boundaries - doublepadding;
            var childsize = Child.Measure(measureCache, available);

            return new SKSize(boundaries.Width, childsize.Height + doublepadding.Height);
        }

        public override SKRect PaintInternal(LayoutSurface layoutsurface, SKRect rect)
        {

            var paintrect = new SKRect(rect.Left + Padding.Width, rect.Top + Padding.Height, rect.Right - Padding.Width, rect.Bottom - Padding.Height);
            //var childrect = new SKRect(rect.Left, rect.Top + Padding.Height, rect.Right, rect.Bottom - Padding.Height);
            var childactual = layoutsurface.Paint(Child, paintrect);

            var actual = new SKRect(rect.Left, rect.Top, rect.Right, rect.Top + childactual.Height + Padding.Height);

            if (layoutsurface.Canvas != null)
            {

                if (BorderColor != null && BorderColor.Alpha != 0)
                    using (var rrect = new SKRoundRect(actual, CornerRadius, CornerRadius))
                    {

                        if (Material)
                        {

                            using (var paint = new SKPaint() { Color = SKColors.Gray.WithAlpha(32), IsAntialias = true })
                                layoutsurface.Canvas.DrawRoundRect(rrect, paint);

                            using (var paint = new SKPaint() { Color = SKColors.Gray })
                                layoutsurface.Canvas.DrawRect(new SKRect(actual.Left, actual.Bottom - 2, actual.Right, actual.Bottom), paint);

                        }
                        else
                        {

                            using (var paint = new SKPaint() { Color = BorderColor, IsStroke = true, IsAntialias = true })
                                layoutsurface.Canvas.DrawRoundRect(rrect, paint);

                        }


                    }



            }

            return actual;
        }

    }
}
