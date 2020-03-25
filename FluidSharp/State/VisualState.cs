using FluidSharp.Touch;
using SkiaSharp;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace FluidSharp.State
{

    public class VisualState
    {

        private ConcurrentDictionary<object, object> values = new ConcurrentDictionary<object, object>();

        private Func<Task> OnStateChanged;

        public VisualState(Func<Task> onStateChanged)
        {
            OnStateChanged = onStateChanged;
        }


        public Task RequestRedraw()
        {
            if (OnStateChanged == null) 
                return Task.CompletedTask;
            else
                return Task.Run(() => OnStateChanged());
        }

        public object this[object index]
        {
            get => values.GetOrAdd(index, id => null);
            set
            {
                var ischanged = false;
                values.AddOrUpdate(index, (i) =>
                {
                    ischanged = true;
                    return value;
                }, (i2, o) =>
                {
                    ischanged = value != o;
                    return value;
                });
                if (ischanged && OnStateChanged != null)
                    RequestRedraw();
            }
        }

        public T GetValue<T>(object index) => (T)this[index];
        public T GetOrDefault<T>(object index) => GetOrMake(index, () => default(T));

        public T GetOrMake<T>(object index, Func<T> make) => (T)values.GetOrAdd(index, (t) => make());


        public TouchTarget TouchTarget
        {
            get => GetOrMake("touchtarget", () => new TouchTarget());
            set => this["touchtarget"] = value;
        }

    }
}
