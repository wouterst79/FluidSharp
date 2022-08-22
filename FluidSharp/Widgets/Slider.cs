using FluidSharp.Layouts;
using FluidSharp.Paint;
using FluidSharp.State;
using FluidSharp.Widgets.CrossPlatform;
using SkiaSharp;
using SkiaSharp.TextBlocks.Enum;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace FluidSharp.Widgets
{
    public class Slider : Widget
    {

        private float LineHeight = 2;
        public float Height;

        public float Min;
        public float Max;
        public float? Value;

        public SKColor ValuePartColor;
        public SKColor OtherPartColor;
        public Widget Thumb;

        public float Width;
        public FlowDirection FlowDirection;

        public Slider(float height, float min, float max, float? value, SKColor valuePartColor, SKColor otherPartColor, Widget thumb)
        {
            Height = height;
            Min = min;
            Max = max;
            Value = value;
            ValuePartColor = valuePartColor;
            OtherPartColor = otherPartColor;
            Thumb = thumb ?? throw new ArgumentNullException(nameof(Thumb));
        }

        public static Widget Make(VisualState visualState, object context, float height, float min, float max, float? value, Func<float, Task> onValueChanged, SKColor valuePartColor, SKColor otherPartColor, PlatformStyle platformStyle)
        {

            Widget thumb;
            if (platformStyle == PlatformStyle.Material)
            {
                var selected = visualState.TouchTarget.IsContext(context);
                if (selected)
                {
                    thumb = new Container(ContainerLayout.Wrap)
                    {
                        Children =
                        {
                            new Circle(30, platformStyle.InkWellColor, SKColors.Transparent),
                            new Center(new Circle(20, valuePartColor, SKColors.Transparent))
                        }
                    };
                }
                else
                    thumb = new Circle(15, valuePartColor, SKColors.Transparent);
            }
            else
            {
                thumb = new Circle(20, SKColors.White, SKColors.Gray) { ImageFilter = platformStyle.DropShadowImageFilterSmall };
            }

            //var state = visualState.GetOrMake("SliderState", () => new SliderState());
            var slider = new Slider(height, min, max, value, valuePartColor, otherPartColor, thumb);

            return new GestureDetector.TouchLocationDetector(visualState, context, (point) =>
            {
                var newvalue = slider.GetValue(point);
                if (newvalue.HasValue && onValueChanged != null)
                    return onValueChanged(newvalue.Value);
                else
                    return Task.CompletedTask;
            }, slider);

        }


        public static Widget Make(VisualState visualState, object context, float height, float min, float max, float value, Func<float, Task> onValueChanged, SKColor valuePartColor, SKColor otherPartColor, Widget thumb)
        {

            //var state = visualState.GetOrMake("SliderState", () => new SliderState());
            var slider = new Slider(height, min, max, value, valuePartColor, otherPartColor, thumb);

            return new GestureDetector.TouchLocationDetector(visualState, context, (point) =>
            {
                var newvalue = slider.GetValue(point);
                if (newvalue.HasValue && onValueChanged != null)
                    return onValueChanged(newvalue.Value);
                else
                    return Task.CompletedTask;
            }, slider);

        }

        public float? GetValue(SKPoint location)
        {
            if (Width > 0)
            {

                var x = location.X;
                if (FlowDirection == FlowDirection.RightToLeft)
                    x = Width - x;

                if (x < 0) x = 0;
                if (x > Width) x = Width;

                var value = Min + (x / Width) * (Max - Min);
                return value;

            }
            return null;
        }

        public override SKSize Measure(MeasureCache measureCache, SKSize boundaries)
        {
            return new SKSize(boundaries.Width, Height);
        }

        public override SKRect PaintInternal(LayoutSurface layoutsurface, SKRect rect)
        {

            Width = rect.Width;
            rect = new SKRect(rect.Left, rect.Top, rect.Right, rect.Top + Height);

            FlowDirection = layoutsurface.Device.FlowDirection;

            var canvas = layoutsurface.Canvas;
            if (canvas != null)
            {

                var liney = rect.Top + (rect.Height - LineHeight) / 2;

                if (Value is null)
                {

                    var x1 = rect.Left;
                    var x2 = rect.Right;

                    // Other part
                    canvas.DrawRect(new SKRect(x1, liney, x2, liney + LineHeight), PaintCache.GetBackgroundPaint(OtherPartColor));

                }
                else
                {

                    var part = (Value.Value - Min) / (Max - Min);
                    var w = rect.Width * (part);

                    var x1 = FlowDirection == FlowDirection.LeftToRight ? rect.Left : rect.Right;
                    var x2 = FlowDirection == FlowDirection.LeftToRight ? rect.Left + w : rect.Right - w;
                    var x3 = FlowDirection == FlowDirection.LeftToRight ? rect.Right : rect.Left;

                    // Value part
                    canvas.DrawRect(new SKRect(x1, liney, x2, liney + LineHeight), PaintCache.GetBackgroundPaint(ValuePartColor));

                    // Other part
                    canvas.DrawRect(new SKRect(x2, liney, x3, liney + LineHeight), PaintCache.GetBackgroundPaint(OtherPartColor));

                    var thumbsize = Thumb.Measure(layoutsurface.MeasureCache, default);

                    var thumby = rect.Top + (rect.Height - thumbsize.Height) / 2;
                    var thumbx = x2 - thumbsize.Width / 2;

                    var thumbrect = SKRect.Create(thumbx, thumby, thumbsize.Width, thumbsize.Height);
                    Thumb.PaintInternal(layoutsurface, thumbrect);

                }

            }

            return rect;
        }

    }
}
