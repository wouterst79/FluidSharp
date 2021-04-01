using FluidSharp.Layouts;
using FluidSharp.State;
using FluidSharp.Widgets;
using SkiaSharp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace FluidSharp.Paint
{

    public class ScrollFinderLayoutSurface : LayoutSurface
    {

        public List<(Scrollable scrollable, float sizedifference)> Scrollables = new List<(Scrollable scrollable, float sizedifference)>();

        public ScrollFinderLayoutSurface(Device device, MeasureCache measureCache) : base(device, measureCache, null, new VisualState(null, null))
        {

        }

        public override SKRect Paint(Widget widget, SKRect rect)
        {
            if (widget is Scrollable scrollable)
            {
                var height = rect.Height;
                var childsize = scrollable.ChildTree.Measure(MeasureCache, rect.Size);
                Scrollables.Add((scrollable, childsize.Height - height));
            }
            return base.Paint(widget, rect);
        }
    }

    public class ScreenshotLayoutSurface : LayoutSurface
    {

        public ScreenshotLayoutSurface(Device device, MeasureCache measureCache, bool showtouchregions) : base(device, measureCache, null, new VisualState(null, null) { ShowTouchRegions = showtouchregions })
        {
        }

        public void Print(Widget widget, string folder, string filename, SKSize size, float scale, bool expandscroll)
        {

            size = new SKSize(size.Width * scale, size.Height * scale);
            widget = new Scale(scale, widget);

            var imageformat = new SKWebpEncoderOptions(SKWebpEncoderCompression.Lossless, 100);
            var fullpath = Path.Combine(folder, filename + ".webp");

            //var imageformat = new SKPngEncoderOptions(SKPngEncoderFilterFlags.AllFilters, 4);
            //var fullpath = Path.Combine(folder, filename + ".png");

            if (expandscroll)
            {

                var finder = new ScrollFinderLayoutSurface(Device, MeasureCache);
                finder.Paint(widget, new SKRect(0, 0, size.Width, size.Height));
                var deltah = finder.Scrollables.Sum(s => s.sizedifference) * scale;
                size = new SKSize(size.Width, size.Height + deltah);

            }


            using (var surface = SKSurface.Create(new SKImageInfo((int)size.Width, (int)size.Height, SKImageInfo.PlatformColorType, SKAlphaType.Premul)))
            {

                //surface.Canvas.Clear(SKColors.Transparent);
                surface.Canvas.Clear(SKColors.White);

                SetCanvas(surface.Canvas);

                Paint(widget, new SKRect(0, 0, size.Width, size.Height));

                if (File.Exists(fullpath))
                    File.Delete(fullpath);

                using (var outstream = new FileStream(fullpath, FileMode.Create))
                {
                    using (var pixmap = surface.PeekPixels())
                        pixmap.Encode(imageformat).SaveTo(outstream);
                }

            }
        }



    }
}
