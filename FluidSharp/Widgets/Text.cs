using FluidSharp.Engine;
using FluidSharp.Layouts;
using SkiaSharp;
using SkiaSharp.TextBlocks;
using SkiaSharp.TextBlocks.Enum;
using System;
using System.Collections.Generic;

namespace FluidSharp.Widgets
{

    public class Text : Widget
    {

        public TextBlock TextBlock;

        public float MarginY => TextBlock.MarginY;
        public int MaxLines { get => TextBlock.MaxLines; set => TextBlock.MaxLines = value; }

        public Text(Font font, SKColor color, string text)
        {
            TextBlock = new TextBlock(font, color, text);
        }

        public Text(Font font, SKColor color, string text, LineBreakMode lineBreakMode)
        {
            TextBlock = new TextBlock(font, color, text, lineBreakMode);
        }


        public override SKSize Measure(MeasureCache measureCache, SKSize boundaries)
        {
            try
            {
                return TextBlock.Measure(boundaries.Width, measureCache.TextShaper);
            }
            catch (Exception ex)
            {
                throw new MeasureException($"unable to measure text: {TextBlock.Text}", ex, new Dictionary<string, string>() { { "Text", TextBlock.Text } });
            }
        }

        public override SKRect PaintInternal(LayoutSurface layoutsurface, SKRect rect)
        {
            try
            {
                return layoutsurface.Canvas.DrawTextBlock(TextBlock, rect, layoutsurface.MeasureCache.TextShaper, layoutsurface.Device.FlowDirection);
            }
            catch (Exception ex)
            {
                throw new PaintException($"unable to paint text: {TextBlock.Text}", ex, new Dictionary<string, string>() { { "Text", TextBlock.Text } });
            }
        }


    }
}
