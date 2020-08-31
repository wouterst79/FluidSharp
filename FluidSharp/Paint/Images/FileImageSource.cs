using SkiaSharp;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace FluidSharp.Paint.Images
{
    public class FileImageSource : ImageSource
    {

        public string Path;

        public FileImageSource(string name, string path) : base(name)
        {
            Path = path;
        }

        public override SKImage GetImage()
        {
            if (File.Exists(Path))
                using (var stream = File.OpenRead(Path))
                {
                    var image = SKImage.FromBitmap(SKBitmap.Decode(stream));
                    return image;
                }

            return null;

        }

    }
}
