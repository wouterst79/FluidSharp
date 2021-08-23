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
    public class FlowSample : Sample
    {

        public override string Name => "Flow";

        private Widget MakeRectangle(float width, SKColor color)
        {
            return new Container(ContainerLayout.FillHorizontal) { MinimumSize = new SKSize(width, 20), Children = { Rectangle.Stroke(color) } };
        }

        public override Widget MakeWidget(VisualState visualState)
        {

            var rectangles = new List<Widget>()
            {
                MakeRectangle(20, SKColors.Red),
                MakeRectangle(30, SKColors.Green),
                MakeRectangle(40, SKColors.Blue),
                MakeRectangle(50, SKColors.Orange),
                MakeRectangle(30, SKColors.Purple),
                MakeRectangle(20, SKColors.Yellow),
                MakeRectangle(60, SKColors.Green),
                MakeRectangle(80, SKColors.Blue),
                MakeRectangle(30, SKColors.Orange),
                MakeRectangle(20, SKColors.Purple),
                MakeRectangle(20, SKColors.Yellow),
            };


            var layout = new Layout()
            {
                ColumnSpacing = 10,
                RowSpacing = 10,
                Margin = new Margins(5),

                Columns =
                {
                    new LayoutSize.Available(.35f)
                },

                Cells =
                {
                    new LayoutCell(0, 0, Rectangle.Stroke(SKColors.LightGray)),
                    new LayoutCell(0, 0, new Flow(){Children = rectangles, Spacing = 10}),

                    new LayoutCell(1, 0, Rectangle.Stroke(SKColors.LightGray)),
                    new LayoutCell(1, 0, new Flow(){Children = rectangles, Spacing = 10}),

                    new LayoutCell(0, 1, Rectangle.Stroke(SKColors.LightGray)),
                    new LayoutCell(0, 1, new Flow(){Children = rectangles, Spacing = 10, Justify = true}),

                    new LayoutCell(1, 1, Rectangle.Stroke(SKColors.LightGray)),
                    new LayoutCell(1, 1, new Flow(){Children = rectangles, Spacing = 10, Justify = true}),
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
