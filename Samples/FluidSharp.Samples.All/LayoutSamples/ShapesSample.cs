using FluidSharp.Layouts;
using FluidSharp.State;
using FluidSharp.Widgets;
using FluidSharp.Widgets.CrossPlatform;
using SkiaSharp;
using SkiaSharp.TextBlocks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FluidSharp.Samples.All.LayoutSamples
{
    public class ShapesSample : Sample
    {

        public override string Name => "Shapes";

        private Widget Print(string text, Widget shape)
        {
            return new Container(ContainerLayout.Fill)
            {
                Children =
                {
                    Rectangle.Stroke(SKColors.Gray),
                    shape,
                    new Text(new Font(14), SKColors.Black, text),
                }
            };
        }

        public override Widget MakeWidget(VisualState visualState)
        {

            var s = 10;
            var LeftArrow = new SKPath(); // ⮜
            LeftArrow.MoveTo(s, 0);
            LeftArrow.LineTo(new SKPoint(0, s / 2));
            LeftArrow.LineTo(new SKPoint(s, s));
            LeftArrow.LineTo(new SKPoint(s - s / 3, s / 2));
            LeftArrow.LineTo(s, 0);

            var layout = new Layout()
            {
                ColumnSpacing = 10,
                RowSpacing = 10,
                Margin = new Margins(5),

                Cells =
                {

                    new LayoutCell(0, 0, Print("Rectangle.Horizontal", Rectangle.Horizontal(20, SKColors.Red))),

                    new LayoutCell(1, 0, Print("Rectangle.Vertical", Rectangle.Vertical(20, SKColors.Blue))),

                    new LayoutCell(2, 0, Print("Rectangle.Fill", Rectangle.Fill(SKColors.Yellow))),

                    new LayoutCell(0, 1, Print("RoundedRectangle", new RoundedRectangle(20, SKColors.LightGray, SKColors.DarkCyan))),

                }

            };

            return new Container(ContainerLayout.Fill)
            {
                Children =
                {
                    new Container(ContainerLayout.Fill) {Margin = new Margins(200)},
                    layout,
                }
            };

        }
    }
}
