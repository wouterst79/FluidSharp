using FluidSharp.Layouts;
using FluidSharp.Paint;
using FluidSharp.State;
using FluidSharp.Widgets.CrossPlatform;
using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace FluidSharp.Widgets
{
    public class Checkbox : Widget
    {

        public enum CheckedState
        {
            Unchecked,
            Checked,
            Mixed
        }

        public CheckedState State;
        public bool Enabled;
        public bool Selected;
        public SKColor ThemeColor;
        public SKColor StrokeColor;
        public SKColor DisabledColor;
        public SKColor SelectedColor;

        // styling
        private static float CornerRadius = 3;
        private static float Size = 15;
        private static float ShapeSize = 8;
        private static float BorderWidth = 1f;
        private static float StrokeWidth = 2;
        private static float LabelSpacing = 7;
        public static float LabelXPosition = Size + LabelSpacing;

        private Checkbox(CheckedState state, bool enabled, bool selected, SKColor themeColor, SKColor strokeColor, SKColor disabledColor, SKColor selectedColor)
        {
            State = state;
            Enabled = enabled;
            Selected = selected;
            ThemeColor = themeColor;
            StrokeColor = strokeColor;
            DisabledColor = disabledColor;
            SelectedColor = selectedColor;
        }

        public static Widget Make(VisualState visualState, object context, CheckedState state, bool enabled, float minimumHeight, Margins margins, PlatformStyle platformStyle, Func<CheckedState, Task> onSetState, Widget label)
        {

            var selected = visualState.TouchTarget.IsContext(context);
            var checkbox = new Checkbox(state, enabled, selected, platformStyle.CheckboxColor, platformStyle.CheckboxStrokeColor, platformStyle.DisabledColor, platformStyle.SelectedColor);

            var contents = new Row(LabelSpacing, VerticalAlignment.Center)
            {
                MinimumHeight = minimumHeight,
                Margin = margins,
                ExpandHorizontal = true,
                Children =
                {
                    checkbox,
                    label
                }
            };

            return GestureDetector.TapDetector(visualState, context, () =>
            {
                if (!enabled) return Task.CompletedTask;
                if (state == CheckedState.Checked)
                    checkbox.State = CheckedState.Unchecked;
                else
                    checkbox.State = CheckedState.Checked;
                return onSetState(checkbox.State);
            }, contents);
        }

        public override SKSize Measure(MeasureCache measureCache, SKSize boundaries)
        {
            return new SKSize(Size, Size);
        }

        public override SKRect PaintInternal(LayoutSurface layoutsurface, SKRect rect)
        {

            var canvas = layoutsurface.Canvas;
            if (canvas != null)
            {

                var rrectcolor = Enabled ? ThemeColor : DisabledColor;
                var rrect = new SKRoundRect(rect, CornerRadius, CornerRadius);

                if (State == CheckedState.Unchecked)
                {

                    // unchecked

                    // solid rrect
                    if (!Enabled)
                        canvas.DrawRoundRect(rrect, PaintCache.GetBackgroundPaint(DisabledColor.WithAlpha(64)));

                    canvas.DrawRoundRect(rrect, PaintCache.GetBorderPaint(StrokeColor, BorderWidth));

                }
                else
                {

                    // checked, mixed

                    // solid rrect
                    canvas.DrawRoundRect(rrect, PaintCache.GetBackgroundPaint(rrectcolor));

                    var path = State == CheckedState.Checked ? GetCheckPath() : GetMixedPath();

                    //var bounds = path.Bounds;
                    using (var drawpath = new SKPath(path))
                    {

                        //drawpath.Transform(SKMatrix.MakeScale(rect.Width / bounds.Width, rect.Height / bounds.Height));
                        drawpath.Transform(SKMatrix.MakeTranslation(rect.MidX - ShapeSize / 2, rect.MidY - ShapeSize / 2));

                        canvas.DrawPath(drawpath, PaintCache.GetBorderPaint(SKColors.White, StrokeWidth));

                    }

                }

                if (Selected)
                {
                    canvas.DrawRoundRect(rrect, PaintCache.GetBackgroundPaint(SelectedColor));
                }

            }


            return rect;
        }

        //✓
        private static SKPath checkpath;
        private static SKPath GetCheckPath()
        {
            if (checkpath == null)
            {

                checkpath = new SKPath();
                checkpath.MoveTo(0, ShapeSize * .6f);
                checkpath.LineTo(ShapeSize * .4f, ShapeSize);
                checkpath.LineTo(ShapeSize, 0);

                var c = checkpath;
                checkpath = null;
                return c;

            }
            return checkpath;
        }


        //✓
        private static SKPath mixedpath;
        private static SKPath GetMixedPath()
        {
            if (mixedpath == null)
            {

                mixedpath = new SKPath();
                mixedpath.MoveTo(0, ShapeSize * .5f);
                mixedpath.LineTo(ShapeSize, ShapeSize * .5f);

                var c = mixedpath;
                mixedpath = null;
                return c;

            }
            return mixedpath;
        }


    }
}
