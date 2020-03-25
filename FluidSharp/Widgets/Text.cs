using FluidSharp.Layouts;
using SkiaSharp;
using SkiaSharp.TextBlocks;
using SkiaSharp.TextBlocks.Enum;

namespace FluidSharp.Widgets
{

    public class Text : Widget
    {

        public TextBlock SKText;

        public float MarginY => SKText.MarginY;
        public int MaxLines { get => SKText.MaxLines; set => SKText.MaxLines = value; }

        public Text(Font font, SKColor color, string text)
        {
            SKText = new TextBlock(font, color, text);
        }

        public Text(Font font, SKColor color, string text, LineBreakMode lineBreakMode) 
        {
            SKText = new TextBlock(font, color, text, lineBreakMode);
        }


        public override SKSize Measure(MeasureCache measureCache, SKSize boundaries)
        {
            return SKText.Measure(boundaries.Width, measureCache.TextShaper);
        }

        public override SKRect PaintInternal(LayoutSurface layoutsurface, SKRect rect)
        {
            return layoutsurface.Canvas.DrawTextBlock(SKText, rect, layoutsurface.MeasureCache.TextShaper, layoutsurface.Device.FlowDirection);
        }


    }
}
