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
    public class AlignSample : Sample
    {

        public override string Name => "Align";

        public override Widget MakeWidget(VisualState visualState)
        {

            var layout = new Layout() { ColumnSpacing = 10, RowSpacing = 10, Margin = new Margins(5) };

            foreach (var h in Enum.GetValues(typeof(HorizontalAlignment)).Cast<HorizontalAlignment>())
            {
                foreach (var v in Enum.GetValues(typeof(VerticalAlignment)).Cast<VerticalAlignment>())
                {

                    layout.Cells.Add(new LayoutCell((int)h, (int)v, new Layout()
                    {

                        Cells =
                        {
                            new LayoutCell(0,0, Rectangle.Stroke(SKColors.LightGray)),
                            new LayoutCell(0,0,
                                new Align(h, v,
                                    new Text(new Font(14), SKColors.Black, $"{h} - {v}")
                                )
                            )
                        }

                    }));

                }
            }

            return new Container(ContainerLayout.Fill)
            {
                Children =
                {
                    new Container(ContainerLayout.Fill) {MinimumSize = new SKSize(200, 200)},
                    layout,
                }
            };

        }
    }
}
