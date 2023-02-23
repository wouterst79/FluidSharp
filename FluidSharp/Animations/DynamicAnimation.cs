using System;
using System.Collections.Generic;
using System.Text;

namespace FluidSharp.Animations
{
    public class DynamicAnimation : IAnimation
    {

        public Animation Animation;

        public DynamicAnimation(Animation animation)
        {
            Animation = animation;
        }

        public float GetValue() => Animation.GetValue();
        public bool Completed => Animation.Completed;

    }
}
