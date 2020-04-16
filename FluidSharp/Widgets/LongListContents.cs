#if DEBUG
//#define DEBUGCONTAINER
#endif
using FluidSharp.Layouts;
using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace FluidSharp.Widgets
{
    public class LongListContents<T> : Widget
    {

#if DEBUGCONTAINER
        public bool Debug;
#endif

        public float Spacing;
        public Widget Separator;


        // Item Access
        public Func<int, T> GetItem;
        public int ItemCount;

        // Item Height: one of these only
        public float? ConstantItemHeight;
        public Func<T, float> GetItemHeight;

        // Make Widget
        public Func<T, Widget> MakeItemWidget;


        public LongListContents(IList<T> items, float itemHeight, Func<T, Task> onItemSelected, Func<T, Widget> makeItemWidget)
        {
            GetItem = (i) => items[i];
            ItemCount = items.Count;
            ConstantItemHeight = itemHeight;
            MakeItemWidget = makeItemWidget;
        }


        public override SKSize Measure(MeasureCache measureCache, SKSize boundaries)
        {

            if (Separator != null)
            {
                var separatorsize = Separator.Measure(measureCache, boundaries);
                Spacing = separatorsize.Height;
            }

            var h = 0f;
            if (ConstantItemHeight.HasValue)
            {
                // constant item height: calculate height
                h = (ConstantItemHeight.Value + Spacing) * ItemCount - Spacing;
            }
            else
            {
                // variable item height: measure all items
                for (int i = 0; i < ItemCount; i++)
                {
                    var item = GetItem(i);
                    h += GetItemHeight(item) + Spacing;
                }
                if (ItemCount > 0)
                    h -= Spacing;
            }

            return new SKSize(boundaries.Width, h);

        }

        public override SKRect PaintInternal(LayoutSurface layoutsurface, SKRect rect)
        {

            if (layoutsurface.Canvas == null)
                return rect;

            var size = Measure(layoutsurface.MeasureCache, rect.Size);
            var cliprect = layoutsurface.Canvas.LocalClipBounds;

            var top = cliprect.Top - rect.Top;
            if (top < 0) top = 0;

            if (Separator != null)
            {
                Spacing = Separator.Measure(layoutsurface.MeasureCache, rect.Size).Height;
            }


            var id = 0;
            var y = 0f;
            if (ConstantItemHeight.HasValue)
            {
                // constant item height: calculate first item and y
                var itemheight = ConstantItemHeight.Value + Spacing;
                id = (int)Math.Floor(top / itemheight);
                y = (ConstantItemHeight.Value + Spacing) * id;
            }
            else
            {
                // variable item height: measure all items
                for (int i = 0; i < ItemCount; i++)
                {
                    var item = GetItem(i);
                    var itemheight = GetItemHeight(item) + Spacing;
                    if (y + itemheight > top)
                        break;
                    y += itemheight;
                    id = i;
                }
            }

            var l = rect.Left;
            var t = rect.Top;
            var r = rect.Right;
            var b = rect.Bottom;


            var height = rect.Height;
            while (id < ItemCount && y < height)
            {

                var item = GetItem(id);

                float h;
                if (ConstantItemHeight.HasValue)
                    h = ConstantItemHeight.Value;
                else
                    h = GetItemHeight(item);


                var childrect = new SKRect(l, t + y, r, t + y + h);

                var child = MakeItemWidget(item);
                var painted = layoutsurface.Paint(child, childrect);

                y += h;

                if (id < ItemCount - 1)
                {
                    if (Separator != null)
                    {
                        layoutsurface.Paint(Separator, new SKRect(l, y, r, y + Spacing));
                    }
                    y += Spacing;
                }

                id++;
            }

#if DEBUGCONTAINER
            if (Debug)
            {
                var measure = Measure(new SKSize(rect.Width, rect.Height));
                layoutsurface.DebugRect(rect, SKColors.Green);
                layoutsurface.DebugRect(new SKRect(rect.Left, rect.Top, rect.Left + measure.Width, rect.Top + measure.Height), SKColors.Red);
            }
#endif

            return new SKRect(l, rect.Top, r, y);

        }

    }
}
