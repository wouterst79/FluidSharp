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
    public class LayoutSample : Sample
    {

        public override string Name => "Layout";

        private Widget Print(string text)
        {
            return new Container(ContainerLayout.Fill)
            {
                Children =
                {
                    Rectangle.Stroke(SKColors.Gray),
                    new Text(new Font(14), SKColors.Black, text),
                }
            };
        }

        public override Widget MakeWidget(VisualState visualState)
        {

            var demo = new Layout()
            {
                Columns =
                {
                    new LayoutSize.Absolute(50),
                    new LayoutSize.Available(.5f),
                    new LayoutSize.Remaining(2),
                },
                Rows =
                {
                    new LayoutSize.Absolute(50),
                    new LayoutSize.Fit(),
                }, 

                Cells =
                {
                    new LayoutCell(0,0, Print("Absolute (50 x 50)")),
                    new LayoutCell(1,0, Print("Available (40%)")),
                    new LayoutCell(2,0, Print("Remaining (2*)")),
                    new LayoutCell(3,0, Print("Remaining (1*)")),
                    new LayoutCell(0,1, 4, 1, Print("Columnspan = 5, Fit")),
                }
            };

            var layout = new Layout()
            {
                ColumnSpacing = 10,
                RowSpacing = 10,
                Margin = new Margins(5),

                Columns =
                {
                    new LayoutSize.Available(.65f)
                },

                Cells =
                {
                    new LayoutCell(0, 0, Rectangle.Stroke(SKColors.LightGray)),
                    new LayoutCell(0, 0, demo),

                    new LayoutCell(0, 1, 2, 1, Rectangle.Stroke(SKColors.LightGray)),
                    new LayoutCell(0, 1, 2, 1, demo),
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
