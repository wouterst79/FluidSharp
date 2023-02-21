using System;
using System.Collections.Generic;
using System.Text;

namespace FluidSharp.Layouts
{
    public class LayoutSize
    {


        private static Dictionary<float, LayoutSize> AvailableLayouts = new Dictionary<float, LayoutSize>();
        public static LayoutSize Available(float part)
        {
            if (!AvailableLayouts.TryGetValue(part, out var layoutSize))
                AvailableLayouts[part] = layoutSize = new Available_Impl(part);
            return layoutSize;
        }

        private static Dictionary<float, LayoutSize> AbsoluteLayouts = new Dictionary<float, LayoutSize>();
        public static LayoutSize Absolute(float size)
        {
            if (!AbsoluteLayouts.TryGetValue(size, out var layoutSize))
                AbsoluteLayouts[size] = layoutSize = new Absolute_Impl(size);
            return layoutSize;
        }

        public static LayoutSize Remaining = new Remaining_Impl(1);

        private static Dictionary<float, LayoutSize> RemainingLayouts = new Dictionary<float, LayoutSize>();
        public static LayoutSize RemainingWithWeight(float weight = 1)
        {
            if (!RemainingLayouts.TryGetValue(weight, out var layoutSize))
                RemainingLayouts[weight] = layoutSize = new Remaining_Impl(weight);
            return layoutSize;
        }


        public static readonly LayoutSize Fit = new Fit_Impl();


        internal class Available_Impl : LayoutSize
        {
            public float Part;
            /// <param name="part">
            /// Percentage.
            /// Note: parts < 1f (100%) are most likely use case
            /// </param>
            public Available_Impl(float part) => Part = part;
        }

        internal class Absolute_Impl : LayoutSize
        {
            public float Size;
            public Absolute_Impl(float size) => Size = size;
        }

        internal class Remaining_Impl : LayoutSize
        {
            public float Weight;
            public Remaining_Impl(float weight = 1) => Weight = weight;
        }

        internal class Fit_Impl : LayoutSize
        {
            public float MinSize;
            public Fit_Impl(float minSize = 0) { MinSize = minSize; }
        }

        public class Hidden : LayoutSize
        {
        }

    }
}
