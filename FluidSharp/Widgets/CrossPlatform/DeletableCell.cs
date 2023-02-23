using FluidSharp.Animations;
using FluidSharp.Layouts;
using FluidSharp.Paint;
using FluidSharp.State;
using FluidSharp.Widgets.Material;
using SkiaSharp;
using SkiaSharp.TextBlocks;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace FluidSharp.Widgets.CrossPlatform
{
    public class DeletableCell
    {

        public static Widget? Make(PlatformStyle platformStyle, VisualState visualState, object context, string deletetext, DateTime created, DateTime? deleted, Func<Task> onTapped, Func<Task> onDelete, Widget? child)
        {

            if (child == null) return null;

            child = HeightTransition.Make(created, deleted, Animation.DefaultDuration, child, true);

            // deleting completed
            if (child == null) return null;

            // ios: sliding cell
            if (platformStyle.DeletableCellIsSliding)
                return SlidingCell.MakeWidget(visualState, context,
                    MakeSlidingCellButton(platformStyle, visualState, context, onTapped, child),
                    null,
                    () => MakeDeleteCellWidget(platformStyle, visualState, deletetext, context, onDelete)
                );

            // others: inkwell
            return new InkWell(ContainerLayout.ExpandHorizontal, visualState, context, platformStyle.InkWellColor, onTapped, onDelete, child);

        }

        /// <summary>
        /// sliding cell button is similar to flat button, with the difference that the background only lights up on tap (and not on touch)
        /// </summary>
        private static Widget MakeSlidingCellButton(PlatformStyle platformStyle, VisualState visualState, object context, Func<Task> onTapped, Widget child)
        {

            var lasttapped = LastTapped.ForContext(visualState, context);
            if (lasttapped.HasValue && lasttapped.Value.Add(Animation.DefaultDuration) > DateTime.Now)
            {
                child = new Container(ContainerLayout.FillHorizontal)
                {
                    Children =
                    {
                        Rectangle.Fill(platformStyle.FlatButtonSelectedBackgroundColor),
                        child
                    }
                };
            }

            Func<Task> onTappedExtended = () => { LastTapped.SetContext(visualState, new TapContext(context)); return onTapped(); };
            return GestureDetector.TapDetector(visualState, context, onTappedExtended, null, child);

        }

        private static Widget MakeDeleteCellWidget(PlatformStyle platformStyle, VisualState visualState, string deletetext, object context, Func<Task> onDelete)
        {
            return new Layout()
            {
                Columns =
                {
                    LayoutSize.Absolute(100),
                },
                Cells =
                {
                    new LayoutCell(0,0, Rectangle.Fill(SKColors.Red)),
                    new LayoutCell(0,0,
                            CrossButton.Make(platformStyle, visualState, new ChildContext(context, "delete"), onDelete,
                                Align.Center(new Text(new Font(14), SKColors.White, deletetext))
                            )
                        ),
                }
            };
        }

    }
}
