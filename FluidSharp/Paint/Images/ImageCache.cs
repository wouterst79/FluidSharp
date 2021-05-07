using FluidSharp.Widgets;
using SkiaSharp;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace FluidSharp.Paint.Images
{
    public class ImageCache
    {

        private ConcurrentDictionary<string, SKImage> imagecache = new ConcurrentDictionary<string, SKImage>();

        private ConcurrentBag<ImageSource> loadqueue = new ConcurrentBag<ImageSource>();

        public Action OnImageLoaded;

        private Task? LoadTask;

        public ImageCache(Action onImageLoaded)
        {
            OnImageLoaded = onImageLoaded;
        }


//#if DEBUG
        public void Remove(Predicate<string> keypredicate)
        {
            foreach (var key in imagecache.Keys)
                if (keypredicate(key))
                    imagecache.Remove(key, out _);
        }
//#endif

        static bool inhere;
        public SKImage? GetImage(ImageSource? source)
        {

            if (source?.Name == null)
                return null;

            // try the cache first
            if (imagecache.TryGetValue(source.Name, out var image))
                return image;

            var loadedimage = source.GetImage();
            imagecache.AddOrUpdate(source.Name, loadedimage, (l, i) => loadedimage);

            return loadedimage;
            //if (inhere)
            //    return null;
            //inhere = true;

            //// add item to queue
            //loadqueue.Add(source);

            //// make sure working process is running
            //if (LoadTask == null)
            //    LoadTask = Task.Run(ProcessQueue);

            //// return null image while image is loading
            //return null;

        }

        async void ProcessQueue()
        {

            await Task.Delay(1).ConfigureAwait(false);

            if (LoadTask == null)
                throw new ArgumentOutOfRangeException();

            while (loadqueue.TryTake(out var source))
            {

                Thread.Sleep(10000);

                if (!imagecache.TryGetValue(source.Name, out var current))
                {

                    var loadedimage = source.GetImage();
                    imagecache.AddOrUpdate(source.Name, loadedimage, (l, i) => loadedimage);

                }

            }

            // request repaint
            if (OnImageLoaded != null)
                await Task.Run(OnImageLoaded);

            LoadTask = null;

        }

    }
}
