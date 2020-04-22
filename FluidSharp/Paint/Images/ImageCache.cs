﻿using SkiaSharp;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace FluidSharp.Paint.Images
{
    public class ImageCache
    {

        private ConcurrentDictionary<string, SKImage> imagecache = new ConcurrentDictionary<string, SKImage>();
        private ConcurrentDictionary<string, Task> loadtasks = new ConcurrentDictionary<string, Task>();

        public Action OnImageLoaded;

        public ImageCache(Action onImageLoaded)
        {
            OnImageLoaded = onImageLoaded;
        }

        public SKImage GetImage(ImageSource source)
        {

            if (source?.Name == null)
                return null;

            // try the cache first
            if (imagecache.TryGetValue(source.Name, out var image))
                return image;

            // escape if an existing load task is running
            if (loadtasks.TryGetValue(source.Name, out var loadtask))
            {
                if (!loadtask.IsCompleted)
                    return null;

                loadtasks.TryRemove(source.Name, out _);
                if (imagecache.TryGetValue(source.Name, out image))
                    return image;

            }

            // start a load task
            loadtask = Task.Run(() =>
            {

                // load the image
                var loadedimage = source.GetImage();

                // store in cache
                imagecache.AddOrUpdate(source.Name, loadedimage, (l, i) => loadedimage);

                // dequeue load task
                loadtasks.TryRemove(source.Name, out _);

                // request repaint
                if (OnImageLoaded != null)
                    Task.Run(OnImageLoaded);

            });
            loadtasks.TryAdd(source.Name, loadtask);

            // return null image while image is loading
            return null;

        }

    }
}