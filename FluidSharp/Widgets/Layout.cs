#if DEBUG
#define DEBUGCONTAINER
#endif
using FluidSharp.Layouts;
using SkiaSharp;
using SkiaSharp.TextBlocks.Enum;
using System;
using System.Collections.Generic;
using System.Text;

namespace FluidSharp.Widgets
{
    public class Layout : Widget
    {

        public bool Debug;

        public Margins Margin;

        public float ColumnSpacing;
        public float RowSpacing;

        public List<LayoutSize> Columns = new List<LayoutSize>();
        public List<LayoutSize> Rows = new List<LayoutSize>();

        public List<LayoutCell> Cells = new List<LayoutCell>();

        public Layout()
        {
        }

        public static Layout Row(params Widget[] widgets)
        {
            var cells = new List<LayoutCell>();
            foreach (var widget in widgets)
                if (widget != null)
                    cells.Add(new LayoutCell(cells.Count, 0, widget));
            return new Layout() { Cells = cells, Rows = { new LayoutSize.Fit() } };
        }

        public static Layout Row(Margins margins, float spacing, params Widget[] widgets)
        {
            var cells = new List<LayoutCell>();
            foreach (var widget in widgets)
                if (widget != null)
                    cells.Add(new LayoutCell(cells.Count, 0, widget));
            return new Layout() { Margin = margins, ColumnSpacing = spacing, Cells = cells, Rows = { new LayoutSize.Fit() } };
        }

        public static Layout Row(Margins margins, Widget separator, float spacing, params Widget[] widgets)
        {
            var cells = new List<LayoutCell>();
            var columns = new List<LayoutSize>();
            var wasnotnull = false;
            foreach (var widget in widgets)
            {
                if (wasnotnull)
                {
                    columns.Add(new LayoutSize.Absolute(spacing));
                    cells.Add(new LayoutCell(cells.Count, 0, separator));
                }
                if (wasnotnull = (widget != null))
                {
                    columns.Add(new LayoutSize.Remaining());
                    cells.Add(new LayoutCell(cells.Count, 0, widget));
                }
            }
            return new Layout() { Margin = margins, ColumnSpacing = spacing, Cells = cells, Columns = columns, Rows = { new LayoutSize.Fit() } };
        }

#if DEBUG

        public static void TestMeasure()
        {

            var boundaries = new SKSize(1000, 1000);

            var abs = new Layout()
            {
                Rows = { new LayoutSize.Absolute(50) },
                Columns = { new LayoutSize.Absolute(50) },
                Cells = { new LayoutCell(0, 0, Rectangle.Fill(SKColors.Black)) }
            };
            var abssize = abs.Measure(null, boundaries);

            System.Diagnostics.Debug.Assert(abssize == new SKSize(50, 50));


            var rel = new Layout()
            {
                Margin = new Margins(10),
                Rows = { new LayoutSize.Remaining() },
                Columns = { new LayoutSize.Remaining() },
                Cells = { new LayoutCell(0, 0, Rectangle.Fill(SKColors.Black)) }
            };
            var relsize = rel.Measure(null, boundaries);

            System.Diagnostics.Debug.Assert(relsize == new SKSize(1000, 20));


            var lc = new Device();
            var ht = new Touch.HitTestLayoutSurface(lc, null, new SKPoint(75, 75), null);
            var rect = new SKRect(0, 0, 100, 100);
            ht.Paint(abs, rect);


            System.Diagnostics.Debug.Assert(ht.Hits.Count == 0);

            ht.Paint(rel, rect);

            System.Diagnostics.Debug.Assert(ht.Hits.Count == 3);


        }

#endif

        public override SKSize Measure(MeasureCache measureCache, SKSize boundaries)
        {

            EnsureColumns();

            var available = new SKSize(boundaries.Width - Margin.TotalX, boundaries.Height - Margin.TotalY);

            // calculate width
            var columns = GetSizes(Columns);
            float w = available.Width + Margin.TotalX;
            if (columns.fit)
            {
                throw new NotImplementedException();
            }
            else if (columns.remaining == 0)
            {
                w = available.Width * columns.available
                    + columns.absolute
                    + (columns.count - 1) * ColumnSpacing
                    + Margin.TotalX;
            }

            // calculate height
            var rows = GetSizes(Rows);
            float h;
            if (rows.fit)
            {

                available.Width -= (Columns.Count - 1) * ColumnSpacing;
                available.Height -= (Rows.Count - 1) * RowSpacing;

                var columnsizes = GetSizes(measureCache, Columns, available.Width, null);
                var rowsizes = GetSizes(measureCache, Rows, available.Height, columnsizes);
                h = Margin.TotalY;
                for (int i = 0; i < rowsizes.Length; i++)
                    h += rowsizes[i];
            }
            else
            {
                h = available.Height * rows.available
                    + rows.absolute
                    + (rows.count - 1) * RowSpacing
                    + Margin.TotalY;
            }

            return new SKSize(w, h);

        }

        public override SKRect PaintInternal(LayoutSurface layoutsurface, SKRect rect)
        {

            if (Debug)
            {
                // place for breakpoint
            }

            EnsureColumns();
            //var size = Measure(layoutsurface.MeasureCache, rect.Size);

            float x;
            if (layoutsurface.Device.FlowDirection == FlowDirection.LeftToRight)
                x = rect.Left + Margin.Near;
            else
                x = rect.Right - Margin.Near;

            var y = rect.Top + Margin.Top;
            var w = rect.Width - Margin.TotalX - (Columns.Count - 1) * ColumnSpacing;

            var availableh = rect.Height - Margin.TotalY - (Rows.Count - 1) * RowSpacing;

            var columns = GetSizes(layoutsurface.MeasureCache, Columns, w, null);
            var rows = GetSizes(layoutsurface.MeasureCache, Rows, availableh, columns);

            var h = 0f;
            for (int i = 0; i < rows.Length; i++)
                h += rows[i];

            foreach (var cell in Cells)
                if (cell != null)
                {


                    // cell x
                    var column = cell.Column;
                    float cx = x;
                    if (layoutsurface.Device.FlowDirection == FlowDirection.LeftToRight)
                        for (int i = 1; i <= column; i++)
                            cx += ColumnSpacing + columns[i - 1];
                    else
                        for (int i = 1; i <= column; i++)
                            cx -= ColumnSpacing + columns[i - 1];

                    // cell width
                    var cw = columns[column];
                    for (int i = 1; i < cell.ColumnSpan; i++)
                        cw += ColumnSpacing + columns[column + i];

                    // cell y
                    var cy = y;
                    var row = cell.Row;
                    for (int i = 1; i <= row; i++)
                        cy += RowSpacing + rows[i - 1];

                    // cell height
                    var ch = rows[row];
                    for (int i = 1; i < cell.RowSpan; i++)
                        ch += RowSpacing + rows[row + i];

                    // assemble child rect
                    SKRect childrect;
                    if (layoutsurface.Device.FlowDirection == FlowDirection.LeftToRight)
                        childrect = new SKRect(cx, cy, cx + cw, cy + ch);
                    else
                        childrect = new SKRect(cx - cw, cy, cx, cy + ch);

                    // paint the child
                    var painted = layoutsurface.Paint(cell, childrect);

                    //layoutsurface.DebugRect(rect, SKColors.Red);

                }

#if DEBUGCONTAINER
            if (Debug)
            {

                var lx = x;
                for (int i = 0; i < columns.Length; i++)
                {
                    if (layoutsurface.Device.FlowDirection == FlowDirection.LeftToRight)
                        lx += columns[i];
                    else
                        lx -= columns[i];
                    layoutsurface.DebugLine(lx, y, lx, y + h, SKColors.Purple.WithAlpha(64));

                    if (ColumnSpacing != 0)
                    {
                        if (layoutsurface.Device.FlowDirection == FlowDirection.LeftToRight)
                            lx += ColumnSpacing;
                        else
                            lx -= ColumnSpacing;
                        layoutsurface.DebugLine(lx, y, lx, y + h, SKColors.Purple.WithAlpha(64));
                    }
                }

                var ly = y;
                for (int i = 0; i < rows.Length; i++)
                {

                    ly += rows[i];
                    layoutsurface.DebugLine(x, ly, x + w, ly, SKColors.Purple.WithAlpha(64));

                    if (RowSpacing != 0)
                    {
                        ly += RowSpacing;
                        layoutsurface.DebugLine(x, ly, x + w, ly, SKColors.Purple.WithAlpha(64));
                    }
                }

                //var measure = Measure(new SKSize(rect.Width, rect.Height));
                //layoutsurface.DebugRect(rect, SKColors.Green);
                //layoutsurface.DebugRect(new SKRect(rect.Left, rect.Top, rect.Left + measure.Width, rect.Top + measure.Height), SKColors.Red);
            }
#endif

            if (layoutsurface.Device.FlowDirection == FlowDirection.LeftToRight)
                return new SKRect(rect.Left, rect.Top, rect.Left + w + Margin.TotalX, rect.Top + h + Margin.TotalY);
            else
                return new SKRect(rect.Right - w - Margin.TotalX, rect.Top, rect.Right, rect.Top + h + Margin.TotalY);

        }

        private void EnsureColumns()
        {

            foreach (var cell in Cells)
                if (cell != null)
                {

                    var columns = cell.Column + cell.ColumnSpan;
                    while (Columns.Count < columns)
                        Columns.Add(new LayoutSize.Remaining());

                    var rows = cell.Row + cell.RowSpan;
                    while (Rows.Count < rows)
                        Rows.Add(new LayoutSize.Remaining());

                }

        }

        private (int count, float available, float absolute, float remaining, bool fit) GetSizes(List<LayoutSize> sizes)
        {

            var count = sizes.Count;
            var available = 0f;
            var absolute = 0f;
            var remaining = 0f;
            var fit = false;


            foreach (var size in sizes)
            {
                if (size is LayoutSize.Available sp) available += sp.Part;
                if (size is LayoutSize.Absolute ss) absolute += ss.Size;
                if (size is LayoutSize.Remaining sr) remaining += sr.Weight;
                if (size is LayoutSize.Fit) fit = true;
            }

            if (available > 1)
                available = 1;

            return (count, available, absolute, remaining, fit);

        }


        private float GetRowSize(MeasureCache measureCache, int row, float[] columns, float availableheight)
        {

            var height = 0f;
            foreach (var cell in Cells)
                if (cell != null)
                {

                    if (cell.Row == row && cell.RowSpan == 1)
                    {

                        // cell x
                        var column = cell.Column;

                        // cell width
                        var cw = columns[column];
                        for (int i = 1; i < cell.ColumnSpan; i++)
                            cw += ColumnSpacing + columns[column + i];

                        var measured = cell.Measure(measureCache, new SKSize(cw, availableheight));
                        if (height < measured.Height)
                            height = measured.Height;

                    }

                }

            return height;
        }

        private float[] GetSizes(MeasureCache measureCache, List<LayoutSize> sizes, float available, float[] columns)
        {

            var result = new float[sizes.Count];
            var totalrelativeweight = 0f;

            var remaining = available;
            for (int i = 0; i < sizes.Count; i++)
            {
                var size = sizes[i];
                if (size is LayoutSize.Available sp)
                {
                    var s = sp.Part * available;
                    result[i] = s;
                    remaining -= s;
                }
                else if (size is LayoutSize.Absolute ss)
                {
                    var s = ss.Size;
                    result[i] = s;
                    remaining -= s;
                }
                else if (size is LayoutSize.Remaining sr)
                    totalrelativeweight += sr.Weight;
                else if (size is LayoutSize.Fit sf)
                {
                    if (columns == null) throw new Exception("fit is not yet supported for columns");
                    var s = GetRowSize(measureCache, i, columns, remaining);
                    result[i] = s;
                    remaining -= s;
                }
                else if (size is null || size is LayoutSize.Hidden)
                    result[i] = 0;
            }

            if (totalrelativeweight > 0)
            {
                for (int i = 0; i < sizes.Count; i++)
                {
                    var size = sizes[i];
                    if (size is LayoutSize.Remaining tr)
                    {
                        result[i] = tr.Weight * remaining / totalrelativeweight;
                    }
                }
            }

            return result;
        }

    }
}
