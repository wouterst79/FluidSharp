using FluidSharp.Layouts;
using SkiaSharp;
using SkiaSharp.TextBlocks;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace FluidSharp.Widgets
{
    public class RichText : Widget, ITextWidget
    {

        public float MarginY;
        public RichTextBlock RichTextBlock = new RichTextBlock();
        public bool Any() => RichTextBlock.Spans.Count > 0;

        public RichText() { }

        public RichText(params Text[] texts)
        {
            foreach (var text in texts)
                AddText(text);
        }

        public RichText(RichTextBlock richTextBlock) { RichTextBlock = richTextBlock; }

        public void AddText(Text text)
        {
            var my = text.GetMarginY();
            if (MarginY < my) MarginY = my;
            RichTextBlock.Add(text.TextBlock);
        }

        public float GetMarginY() => MarginY;

        public override SKSize Measure(MeasureCache measureCache, SKSize boundaries)
        {
            return RichTextBlock.Measure(boundaries.Width, measureCache.TextShaper);
        }

        public override SKRect PaintInternal(LayoutSurface layoutsurface, SKRect rect)
        {
            var result = RichTextBlock.Paint(layoutsurface.Canvas, rect, layoutsurface.Device.FlowDirection, layoutsurface.MeasureCache.TextShaper);

            //#if DEBUG
            //            var measured = Measure(layoutsurface.MeasureCache, rect.Size);
            //            if (result.Height > measured.Height)
            //            {
            //                System.Diagnostics.Debug.WriteLine($"warning: Paint resulted in larger rect {result} than measured rect {measured}");
            //            }
            //#endif

            return result;
        }

    }
}
