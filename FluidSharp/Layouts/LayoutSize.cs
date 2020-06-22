using System;
using System.Collections.Generic;
using System.Text;

namespace FluidSharp.Layouts
{
    public class LayoutSize
    {

        public class Available : LayoutSize
        {
            public float Part;
            /// <param name="part">
            /// Percentage.
            /// Note: parts < 1f (100%) are most likely use case
            /// </param>
            public Available(float part) => Part = part;
        }

        public class Absolute : LayoutSize
        {
            public float Size;
            public Absolute(float size) => Size = size;
        }

        public class Remaining : LayoutSize
        {
            public float Weight;
            public Remaining(float weight = 1) => Weight = weight;
        }

        public class Fit : LayoutSize
        {
        }

        public class Hidden : LayoutSize
        {
        }

    }
}
