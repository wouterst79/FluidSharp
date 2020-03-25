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

        public Widget Child;

        public LayoutCell(int column, int row, Widget child) : this(column, row, 1, 1, child)
        {
        }

        public LayoutCell(int column, int row, int columnSpan, int rowSpan, Widget child)
        {
            Column = column;
            Row = row;
            ColumnSpan = columnSpan;
            RowSpan = rowSpan;
            Child = child;
        }

        public override SKSize Measure(MeasureCache measureCache, SKSize boundaries) => Child.Measure(measureCache, boundaries);
        public override SKRect PaintInternal(LayoutSurface layoutsurface, SKRect rect)
        {
            var result = layoutsurface.Paint(Child, rect);
            if (Debug)
                layoutsurface.DebugRect(rect, SKColors.Red);
            return result;
        }
    }
}
