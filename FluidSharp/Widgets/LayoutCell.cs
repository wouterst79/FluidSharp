using FluidSharp.Layouts;
using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Text;

namespace FluidSharp.Widgets
{
    public class LayoutCell : Widget
    {

        public bool Debug;

        public int Column;
        public int Row;

        public int ColumnSpan;
        public int RowSpan;

        public Widget? Child;

        public HorizontalAlignment HorizontalAlignment;

        public LayoutCell(int column, int row, Widget? child, HorizontalAlignment horizontalAlignment = HorizontalAlignment.Near) : this(column, row, 1, 1, child, horizontalAlignment)
        {
        }

        public LayoutCell(int column, int row, int columnSpan, int rowSpan, Widget? child, HorizontalAlignment horizontalAlignment = HorizontalAlignment.Near)
        {
            Column = column;
            Row = row;
            ColumnSpan = columnSpan;
            RowSpan = rowSpan;
            Child = child;
            HorizontalAlignment = horizontalAlignment;
        }

        public override SKSize Measure(MeasureCache measureCache, SKSize boundaries) => Child?.Measure(measureCache, boundaries) ?? new SKSize();
        public override SKRect PaintInternal(LayoutSurface layoutsurface, SKRect rect)
        {
            if (Child == null)
                return new SKRect(rect.Left, rect.Top, rect.Right, rect.Top);

            if (HorizontalAlignment == HorizontalAlignment.Near)
            {
                // near, no measure needed
                var result = layoutsurface.Paint(Child, rect);
                if (Debug)
                    layoutsurface.DebugRect(rect, SKColors.Red);
                return result;
            }
            else
            {
                // center or far
                var childsize = Child.Measure(layoutsurface.MeasureCache, rect.Size);
                var childrect = rect.HorizontalAlign(childsize, HorizontalAlignment, layoutsurface.Device.FlowDirection);

                var result = layoutsurface.Paint(Child, childrect);
                if (Debug)
                    layoutsurface.DebugRect(childrect, SKColors.Red);
                return result;
            }

        }
    }
}
