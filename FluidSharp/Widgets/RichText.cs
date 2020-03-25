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
    public class RichText : Widget
    {

        public List<Text> Text = new List<Text>();

        private RichTextBlock RichTextBlock;

        private void LoadRichTextBlock()
        {
            if (RichTextBlock != null) return;
            RichTextBlock = new RichTextBlock();
            foreach (var text in Text)
                RichTextBlock.Spans.Add(text.TextBlock);
        }

        public override SKSize Measure(MeasureCache measureCache, SKSize boundaries)
        {
            LoadRichTextBlock();
            return RichTextBlock.Measure(boundaries.Width, measureCache.TextShaper);
        }

        public override SKRect PaintInternal(LayoutSurface layoutsurface, SKRect rect)
        {
            LoadRichTextBlock();
            return RichTextBlock.Paint(layoutsurface.Canvas, rect, layoutsurface.Device.FlowDirection, layoutsurface.MeasureCache.TextShaper);
        }

    }
}
