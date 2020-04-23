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

        public List<Text> Text = new List<Text>();

        protected RichTextBlock RichTextBlock;

        public float GetMarginY()
        {
            var max = 0f;
            foreach (var text in Text)
                if (text != null && text.GetMarginY() > max) max = text.GetMarginY();
            return max;
        }

        private void LoadRichTextBlock()
        {
            if (RichTextBlock != null) return;
            RichTextBlock = new RichTextBlock();
            foreach (var text in Text)
                if (text != null)
                    RichTextBlock.Spans.Add(new RichTextSpan() { TextBlock = text.TextBlock });
        }

        public override SKSize Measure(MeasureCache measureCache, SKSize boundaries)
        {
            LoadRichTextBlock();
            return RichTextBlock.Measure(boundaries.Width, measureCache.TextShaper);
        }

        public override SKRect PaintInternal(LayoutSurface layoutsurface, SKRect rect)
        {
            LoadRichTextBlock();
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
