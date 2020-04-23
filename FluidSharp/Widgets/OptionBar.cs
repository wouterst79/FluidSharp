using FluidSharp.Layouts;
using FluidSharp.State;
using FluidSharp.Widgets.CrossPlatform;
using FluidSharp.Widgets.Material;
using SkiaSharp;
using SkiaSharp.TextBlocks.Enum;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace FluidSharp.Widgets
{
    public class OptionBar : Widget
    {

        public object Context;
        public Widget Background;
        public SKSize Padding;
        public Widget Separator;
        public Widget ButtonBackground;
        public List<Widget> Buttons;

        public OptionBarState OptionBarState;

        public OptionBar(object context, Widget background, SKSize padding, Widget separator, Widget buttonBackground, List<Widget> buttons, OptionBarState optionBarState)
        {
            Context = context;
            Background = background;
            Padding = padding;
            Separator = separator;
            ButtonBackground = buttonBackground;
            Buttons = buttons;
            OptionBarState = optionBarState;
        }


        public static OptionBar Make<T>(VisualState visualState, PlatformStyle platformStyle, object context, IEnumerable<T> options, Func<T, bool> isSelected, Func<T, Task> setValue, Func<T, SKColor, Widget> makeButton)
        {
            if (platformStyle == PlatformStyle.Cupertino)
            {
                var background = new RoundedRectangle(10, SKColors.LightGray.WithAlpha(64), default);
                var padding = new SKSize(3, 3);
                var separator = Rectangle.Vertical(2, SKColors.Gray.WithAlpha(128), new Margins(0, 5));
                var buttonBackground = new RoundedRectangle(8, SKColors.White, default) { ImageFilter = platformStyle.DropShadowImageFilterSmall };

                Func<T, Widget> makeButton2 = (v) =>
                        GestureDetector.TapDetector(visualState, context, () => setValue(v), null, 
                            new Container(ContainerLayout.Fill,
                                Align.Center(
                                    makeButton(v, SKColors.Black)
                                    , new SKSize(10, 3)
                                )
                            )
                        );

                return Make(visualState, context, background, padding, separator, buttonBackground, options, isSelected, makeButton2);
            }
            else
            {

                Widget background = null;
                SKSize padding = default;
                Widget separator = null;
                
                var themecolor = SKColors.DarkBlue;

                var buttonBackground = Align.Bottom(Rectangle.Horizontal(2, themecolor));
                Func<T, Widget> makeButton2 = (v) =>
                        {

                            var selected = isSelected(v);

                            return new InkWell(ContainerLayout.Wrap, visualState, v, platformStyle.InkWellColor, () => setValue(v),
                                new Container(ContainerLayout.Fill)
                                {
                                    Children =
                                    {
                                        Align.Center(
                                            makeButton(v, selected ? themecolor : SKColors.Black)
                                            , new SKSize(10, 6)
                                        ),
                                        selected ? Rectangle.Fill(themecolor.WithAlpha(32)) : null,
                                    }
                                }
                            );
                        };

                return Make(visualState, context, background, padding, separator, buttonBackground, options, isSelected, makeButton2);
            }
        }

        public static OptionBar Make<T>(VisualState visualState, object context, Widget background, SKSize padding, Widget separator, Widget buttonBackground, IEnumerable<T> options, Func<T, bool> isSelected, Func<T, Widget> makeButton)
        {

            var buttons = new List<Widget>();
            int selected = 0;
            foreach (var option in options)
            {

                var button = makeButton(option);

                if (isSelected(option))
                    selected = buttons.Count;

                buttons.Add(button);
            }

            var optionBarState = visualState.GetOrMake(context, () => new OptionBarState(selected));
            optionBarState.SetCurrent(selected);

            return new OptionBar(context, background, padding, separator, buttonBackground, buttons, optionBarState);

        }

        public override SKSize Measure(MeasureCache measureCache, SKSize boundaries)
        {

            // remove null buttons
            var i = 0;
            while (i < Buttons.Count)
                if (Buttons[i] == null) Buttons.RemoveAt(i); else i++;

            if (Buttons.Count == 0) return new SKSize(boundaries.Width, 0);

            var h = 0f;
            var available = new SKSize((boundaries.Width - Padding.Width * 2) / Buttons.Count, boundaries.Height);
            foreach (var button in Buttons)
            {
                var buttonsize = button.Measure(measureCache, available);
                if (h < buttonsize.Height) h = buttonsize.Height;
            }

            return new SKSize(boundaries.Width, h + Padding.Height * 2);

        }

        public override SKRect PaintInternal(LayoutSurface layoutsurface, SKRect rect)
        {

            var size = Measure(layoutsurface.MeasureCache, rect.Size);
            var backrect = new SKRect(rect.Left, rect.Top, rect.Right, rect.Top + size.Height);

            // paint background
            if (layoutsurface.Canvas != null && Background != null)
                layoutsurface.Paint(Background, backrect);

            if (Buttons.Count > 0)
            {

                var innerrect = new SKRect(backrect.Left + Padding.Width, backrect.Top + Padding.Height, backrect.Right - Padding.Width, backrect.Bottom - Padding.Height);
                var buttonw = innerrect.Width / Buttons.Count;

                if (layoutsurface.Canvas != null)
                {

                    var (scroll, animating) = OptionBarState.GetScroll();

                    // paint separators
                    if (Separator != null)
                    {
                        var separatorhalfwidth = Separator.Measure(layoutsurface.MeasureCache, default).Width / 2;
                        var refscroll = layoutsurface.Device.FlowDirection == FlowDirection.LeftToRight ? scroll : Buttons.Count - scroll - 1;
                        for (int i = 1; i < Buttons.Count; i++)
                        {
                            if (refscroll + 1 < i || refscroll > i)
                            {
                                var x = innerrect.Left + i * buttonw;
                                Separator.PaintInternal(layoutsurface, new SKRect(x - separatorhalfwidth, innerrect.Top, x + separatorhalfwidth, innerrect.Bottom));
                            }
                        }
                    }

                    // paint scroll background
                    var scrollx = scroll * buttonw;
                    if (animating)
                        layoutsurface.SetHasActiveAnimations();

                    SKRect buttonbackrect;
                    if (layoutsurface.Device.FlowDirection == FlowDirection.LeftToRight)
                        buttonbackrect = new SKRect(innerrect.Left + scrollx, innerrect.Top, innerrect.Left + scrollx + buttonw, innerrect.Bottom);
                    else
                        buttonbackrect = new SKRect(innerrect.Right - scrollx - buttonw, innerrect.Top, innerrect.Right - scrollx, innerrect.Bottom);

                    layoutsurface.Paint(ButtonBackground, buttonbackrect);

                }

                // paint buttons
                for (int i = 0; i < Buttons.Count; i++)
                {

                    var buttonx = i * buttonw;

                    SKRect buttonbackrect;
                    if (layoutsurface.Device.FlowDirection == FlowDirection.LeftToRight)
                        buttonbackrect = new SKRect(innerrect.Left + buttonx, innerrect.Top, innerrect.Left + buttonx + buttonw, innerrect.Bottom);
                    else
                        buttonbackrect = new SKRect(innerrect.Right - buttonx - buttonw, innerrect.Top, innerrect.Right - buttonx, innerrect.Bottom);

                    var button = Buttons[i];
                    layoutsurface.Paint(button, buttonbackrect);
                }

            }

            return backrect;

        }

    }
}
