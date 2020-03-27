using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;

namespace FluidSharp.Paint.Images
{

    public abstract class ImageSource
    {

        public readonly string Name;

        public abstract SKImage GetImage();

        public ImageSource(string name) => Name = name;

        //public static ImageSource FromFile(string file);
        //public static ImageSource FromResource(string resource, Type resolvingType);
        //public static ImageSource FromResource(string resource, Assembly sourceAssembly = null);
        //public static ImageSource FromStream(Func<Stream> stream);


        [MethodImpl(MethodImplOptions.NoInlining)]
        public static implicit operator ImageSource(string resource) => new ResourceImageSource(resource, Assembly.GetCallingAssembly());

    }

}
