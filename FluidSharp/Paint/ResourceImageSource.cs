using SkiaSharp;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace FluidSharp.Paint
{
    public class ResourceImageSource : IImageSource
    {

        private Assembly Assembly;
        private string[] ResourceNames;

        private ConcurrentDictionary<string, SKImage> imagecache = new ConcurrentDictionary<string, SKImage>();

        public ResourceImageSource(Type typesource)
        {
            Assembly = typesource.Assembly;
            ResourceNames = Assembly.GetManifestResourceNames();
        }

        public SKImage GetImage(string name)
        {
            if (!imagecache.TryGetValue(name, out var image))
            {
                var fullname = ResourceNames.FirstOrDefault(f => f.EndsWith(name));
                if (fullname == null) throw new Exception($"Resource not found: {name}");

                using (var stream = Assembly.GetManifestResourceStream(fullname))
                {
                    image = SKImage.FromBitmap(SKBitmap.Decode(stream));
                    imagecache.TryAdd(name, image);
                }

            }
            return image;
        }
    }
}
