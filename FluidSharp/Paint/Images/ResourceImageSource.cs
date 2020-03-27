using SkiaSharp;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace FluidSharp.Paint.Images
{
    public class ResourceImageSource : ImageSource
    {

        private Assembly Assembly;

        [MethodImpl(MethodImplOptions.NoInlining)]
        public ResourceImageSource(string name) : base(name)
        {
            Assembly = Assembly.GetCallingAssembly();
        }

        public ResourceImageSource(string name, Assembly assembly) : base(name)
        {
            Assembly = assembly;
        }

        public override SKImage GetImage()
        {

            var resourcenames = Assembly.GetManifestResourceNames();
            var fullname = resourcenames.FirstOrDefault(f => f.EndsWith(Name));

            if (fullname == null)
                throw new Exception($"Resource not found: {Name}");

            using (var stream = Assembly.GetManifestResourceStream(fullname))
            {
                var image = SKImage.FromBitmap(SKBitmap.Decode(stream));
                return image;
            }

        }

    }
}
