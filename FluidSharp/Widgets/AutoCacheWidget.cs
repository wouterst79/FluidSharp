#if false
using FluidSharp.Layouts;
using SkiaSharp;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;

namespace FluidSharp.Widgets
{

    public class WidgetCache
    {

        public class CachedItem
        {
            public string? State;
            public SKRect InRect;
            public SKRect OutRect;
            //public SKDrawable Picture;
            //public SKPicture Picture;
            public SKBitmap Picture;

            public CachedItem(string? state, SKRect inRect, SKRect outRect, SKBitmap picture)
            //public CachedItem(string? state, SKRect inRect, SKRect outRect, SKPicture picture)
            //public CachedItem(string? state, SKRect inRect, SKRect outRect, SKDrawable picture)
            {
                State = state;
                InRect = inRect;
                OutRect = outRect;
                Picture = picture ?? throw new ArgumentNullException(nameof(picture));
            }
        }

        public ConcurrentDictionary<string, CachedItem> Cache = new ConcurrentDictionary<string, CachedItem>();

    }

    public class AutoCacheWidget : Widget
    {

        public string Key;
        public WidgetCache Cache;
        public string? State;
        public Widget? Contents;

        public AutoCacheWidget(string key, WidgetCache cache, string? state, Widget? contents)
        {
            Key = key ?? throw new ArgumentNullException(nameof(key));
            Cache = cache ?? throw new ArgumentNullException(nameof(cache));
            State = state;
            Contents = contents;
        }

        public override SKSize Measure(MeasureCache measureCache, SKSize boundaries) => Contents.Measure(measureCache, boundaries);

        public override SKRect PaintInternal(LayoutSurface layoutsurface, SKRect rect)
        {

            if (Contents == null) return rect.WithHeight(0);

            if (layoutsurface.Canvas == null)
                return layoutsurface.Paint(Contents, rect);

            //if (!layoutsurface.ClipRect.IntersectsWithInclusive(rect))
            //{
            //  return new SKRect(0, 0, 0, 0);
            //}

            //var state = layoutsurface.VisualState;

            //var cache = state.GetOrMake("picturecache", () => new ConcurrentDictionary<object, Picture>());
            //var key = JsonConvert.SerializeObject(Child);

            var cache = Cache.Cache;
            if (cache.TryGetValue(Key, out var item))
            {
                if (item.State != State)
                {
                    //item.Picture.Dispose();
                    System.Diagnostics.Debug.WriteLine($"resetting picture cache: {Key}  ({item.State} != {State})");
                    item = null;
                }
                //if (picture.rect.Size != rect.Size)// || picture.key != key)
                //  picture = null;
            }

            if (item == null)
            {

                //using (var recorder = new SKPictureRecorder())
                //{

                //    var originalcanvas = layoutsurface.Canvas;
                //    layoutsurface.SetCanvas(recorder.BeginRecording(rect));

                //    var outrect = layoutsurface.Paint(Contents, rect);

                //    var recorded = recorder.EndRecordingAsDrawable().Snapshot();
                //    layoutsurface.SetCanvas(originalcanvas);

                //    item = new WidgetCache.CachedItem(State, rect, outrect, recorded);

                //}

                using (var surface = SKSurface.Create(new SKImageInfo((int)rect.Width, (int)rect.Height, SKImageInfo.PlatformColorType, SKAlphaType.Opaque)))
                {

                    surface.Canvas.Clear(SKColors.Transparent);

                    var originalcanvas = layoutsurface.Canvas;
                    layoutsurface.SetCanvas(surface.Canvas);

                    var outrect = layoutsurface.Paint(Contents, rect);

                    using (var pixmap = surface.PeekPixels())
                    {
                        var recorded = SKBitmap.Decode(pixmap.Encode(new SKWebpEncoderOptions(SKWebpEncoderCompression.Lossless, 1)));
                        item = new WidgetCache.CachedItem(State, rect, outrect, recorded);
                    }

                    layoutsurface.SetCanvas(originalcanvas);

                }

                cache[Key] = item;
                //return picture.outrect;

            }


            var skpicture = item.Picture;
            //if (layoutsurface.ClipRect.IntersectsWithInclusive(rect))
            //{
            //    layoutsurface.Canvas.DrawDrawable(skpicture, 0, 0);
            //layoutsurface.Canvas.DrawPicture(skpicture, 0, 0);
            layoutsurface.Canvas.DrawBitmap(skpicture, 0, 0);
            //layoutsurface.DebugRect(item.OutRect, SKColors.Purple);
            //layoutsurface.DebugRect(skpicture.Bounds, SKColors.Purple);
            //}

            return item.OutRect;

        }

    }
}

#endif