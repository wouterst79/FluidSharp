using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace FluidSharp.Widgets.Caching
{

    public class WidgetCache
    {

        private object Parameters = nullobj;
        private Widget? Widget;

        private static object nullobj = new object();

        public void Reset() => Widget = null;

        public Widget Get<T>(T p1, Func<Widget> make)
        {
            if (!TryGet(p1, out var result)) Set(p1, result = make());
            return result;
        }

        public Widget Get<T, U>(T p1, U p2, Func<Widget> make)
        {
            if (!TryGet(p1, p2, out var result)) Set(p1, p2, result = make());
            return result;
        }

        public Widget Get<T, U, V>(T p1, U p2, V p3, Func<Widget> make)
        {
            if (!TryGet(p1, p2, p3, out var result)) Set(p1, p2, p3, result = make());
            return result;
        }

        public Widget Get<T, U, V, W>(T p1, U p2, V p3, W p4, Func<Widget> make)
        {
            if (!TryGet(p1, p2, p3, p4, out var result)) Set(p1, p2, p3, p4, result = make());
            return result;
        }

        public bool TryGet<T>(T p1, [MaybeNullWhen(false)] out Widget widget) => TryGetImpl(p1 ?? nullobj, out widget);
        public void Set<T>(T p1, Widget widget) => SetImpl(p1 ?? nullobj, widget);

        public bool TryGet<T, U>(T p1, U p2, [MaybeNullWhen(false)] out Widget widget) => TryGetImpl((p1, p2), out widget);
        public void Set<T, U>(T p1, U p2, Widget widget) => SetImpl((p1, p2), widget);

        public bool TryGet<T, U, V>(T p1, U p2, V p3, [MaybeNullWhen(false)] out Widget widget) => TryGetImpl((p1, p2, p3), out widget);
        public void Set<T, U, V>(T p1, U p2, V p3, Widget widget) => SetImpl((p1, p2, p3), widget);

        public bool TryGet<T, U, V, W>(T p1, U p2, V p3, W p4, [MaybeNullWhen(false)] out Widget widget) => TryGetImpl((p1, p2, p3, p4), out widget);
        public void Set<T, U, V, W>(T p1, U p2, V p3, W p4, Widget widget) => SetImpl((p1, p2, p3, p4), widget);

        private bool TryGetImpl(object parameters, [MaybeNullWhen(false)] out Widget widget)
        {
            if (Widget != null && Parameters.Equals(parameters))
            {
                widget = Widget;
                return true;
            }
            else
            {
                widget = default;
                return false;
            }
        }

        private void SetImpl(object parameters, Widget widget)
        {
            Parameters = parameters;
            Widget = widget;
        }

    }

}
