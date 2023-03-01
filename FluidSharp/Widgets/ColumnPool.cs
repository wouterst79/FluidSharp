#define SHOWSPACING
#if DEBUG
//#define DEBUGCONTAINER
#endif
using FluidSharp.Layouts;
using FluidSharp.Widgets.Members;
using SkiaSharp;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;

namespace FluidSharp.Widgets
{
    public class ColumnPool : Widget
    {

#if DEBUGCONTAINER
        public bool Debug;
#endif
#if SHOWSPACING
        public SKColor SpacingColor = SKColors.Purple;
#endif

        public Margins Margin;

        public float Spacing;
        public Widget? Separator;
        public bool ExpandHorizontal = true;
        public HorizontalAlignment HorizontalAlignment = HorizontalAlignment.Near;

        private ConcurrentBag<FixableList<Widget?>> BuildPool = new ConcurrentBag<FixableList<Widget?>>();
        private ConcurrentQueue<FixableList<Widget?>> PaintList = new ConcurrentQueue<FixableList<Widget?>>();

        private FixableList<Widget?> BackupList = new FixableList<Widget?>();

        public DateTime LastBuilt;

        public ColumnPool(float spacing = 0, HorizontalAlignment horizontalAlignment = HorizontalAlignment.Near)
        {
            Spacing = spacing;
            HorizontalAlignment = horizontalAlignment;
        }

        public FixableList<Widget?> Start()
        {
            while (BuildPool.TryTake(out var result))
                if (!result.IsFixed)
                    return result;
            return new FixableList<Widget?>();
        }

#if DEBUG
        public bool IsBuilt;
#endif
        public void Build(FixableList<Widget?> contents)
        {
#if DEBUG
            IsBuilt = true;
            if (contents.IsFixed)
            {
                Console.WriteLine("Fixed");
            }
#endif
            LastBuilt = DateTime.Now;
            PaintList.Enqueue(contents);
        }

        private FixableList<Widget?> GetPaintList()
        {
#if DEBUG
            if (!IsBuilt) throw new Exception("ColumnPpol was never completed");
#endif
            var backup = BackupList;

            var lastlist = backup;
            while (PaintList.TryDequeue(out var list))
            {
                if (lastlist != backup)
                {
                    lastlist.IsFixed = false;
                    lastlist.Clear();
                    BuildPool.Add(lastlist);
                }
                lastlist = list;
            }

            BackupList = lastlist;

            lastlist.IsFixed = true;
            return lastlist;
        }

        public override SKSize Measure(MeasureCache measureCache, SKSize boundaries)
        {

            boundaries = Margin.Shrink(boundaries);

            var w = 0f;
            var h = 0f;

            if (Separator != null)
            {
                var separatorsize = Separator.Measure(measureCache, boundaries);
                if (w < separatorsize.Width) w = separatorsize.Width;
                Spacing = separatorsize.Height;
            }

            var Children = GetPaintList();

            var any = false;
            foreach (var child in Children)
            {
                if (child is Spacing s)
                {
                    if (any)
                        h = h - Spacing;
                    h += s.Size.Height;
                }
                else
                if (child != null)
                {
                    var childsize = child.Measure(measureCache, boundaries);
                    if (w < childsize.Width) w = childsize.Width;
                    h += childsize.Height + Spacing;
                    any = true;
                }
            }

            if (any)
                h -= Spacing;

            PaintList.Enqueue(Children);

            if (ExpandHorizontal)
                w = boundaries.Width;

            return Margin.Grow(new SKSize(w, h));

        }

        public override SKRect PaintInternal(LayoutSurface layoutsurface, SKRect rect)
        {

            rect = Margin.Shrink(rect, layoutsurface.FlowDirection);

            var Children = GetPaintList();

            Widget? lastchild = null;
            if (Separator != null)
                Spacing = Separator.Measure(layoutsurface.MeasureCache, rect.Size).Height;

            foreach (var child in Children)
                if (child != null)
                    lastchild = child;

            var l = rect.Left;
            var y = rect.Top;
            var r = rect.Right;
            var b = rect.Bottom;

            var hadchild = false;
            foreach (var child in Children)
            {
                if (child is Spacing s)
                {

                    if (hadchild)
                        y = y - Spacing;

#if SHOWSPACING
                    layoutsurface.DebugSpacing(new SKRect(l, y, r, y + s.Size.Height), s.Size.Height, SKColors.Blue);
#endif

                    y += s.Size.Height;

                }
                else
                if (child != null)
                {

                    var childrect = new SKRect(l, y, r, b);


                    if (HorizontalAlignment != HorizontalAlignment.Near)
                    {
                        var childsize = child.Measure(layoutsurface.MeasureCache, childrect.Size);
                        childrect = childrect.HorizontalAlign(childsize, HorizontalAlignment, layoutsurface.Device.FlowDirection);
                    }

                    var painted = layoutsurface.Paint(child, childrect);

                    y += painted.Height;

                    if (child != lastchild)
                    {
                        if (Separator != null && child != lastchild)
                        {
                            layoutsurface.Paint(Separator, new SKRect(l, y, r, y + Spacing));
                        }

#if SHOWSPACING
                        var idx = Children.IndexOf(child);
                        if (!(Children[idx + 1] is Spacing))
                            layoutsurface.DebugSpacing(new SKRect(l, y, r, y + Spacing), Spacing, SpacingColor);
#endif

                        y += Spacing;
                    }

                    hadchild = true;

                }
            }

            PaintList.Enqueue(Children);

#if DEBUGCONTAINER
            if (Debug)
            {
                var measure = Measure(new SKSize(rect.Width, rect.Height));
                layoutsurface.DebugRect(rect, SKColors.Green);
                layoutsurface.DebugRect(new SKRect(rect.Left, rect.Top, rect.Left + measure.Width, rect.Top + measure.Height), SKColors.Red);
            }
#endif

#if SHOWSPACING
            layoutsurface.DebugMargin(new SKRect(l, rect.Top, r, y), Margin, SKColors.DarkCyan);
#endif

            return Margin.Grow(new SKRect(l, rect.Top, r, y), layoutsurface.FlowDirection);

        }

    }
}
