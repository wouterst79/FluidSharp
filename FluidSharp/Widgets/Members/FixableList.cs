using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace FluidSharp.Widgets.Members
{
    public class FixableList<T> : IList<T>
    {

        private readonly List<T> _list;


        public FixableList() { _list = new List<T>(); }
        public FixableList(IEnumerable<T> values) { _list = new List<T>(values); }


        public bool IsFixed;

        public int IndexOf(T item) => _list.IndexOf(item);

        // fixable API:
        public void Insert(int index, T item) { if (!IsFixed) _list.Insert(index, item); }
        public void RemoveAt(int index) { if (!IsFixed) _list.RemoveAt(index); }
        public T this[int index] { get => _list[index]; set { if (!IsFixed) _list[index] = value; } }
        public void Add(T item) { if (!IsFixed) _list.Add(item); }
        public void Clear() { if (!IsFixed) _list.Clear(); }

        public bool Remove(T item)
        {
            if (IsFixed) return false;
            else return _list.Remove(item);
        }

        // other List APIs (that are not IList)
        public void AddRange(IEnumerable<T> range) { if (!IsFixed) _list.AddRange(range); }


        public bool Contains(T item) => _list.Contains(item);
        public void CopyTo(T[] array, int arrayIndex) => _list.CopyTo(array, arrayIndex);

        public int Count => _list.Count;

        public bool IsReadOnly => IsFixed;

        public IEnumerator<T> GetEnumerator() => _list.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => _list.GetEnumerator();

    }


    public static class FixableListExtensions
    {

        public static FixableList<T?> ToFixableList<T>(this IEnumerable<T> values) where T : class => new FixableList<T?>(values);
        public static FixableList<T> ToFixableList2<T>(this IEnumerable<T> values) => new FixableList<T>(values);

    }

}
