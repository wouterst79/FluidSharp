using System;
using System.Collections.Generic;
using System.Text;

namespace FluidSharp.Animations
{
    public class DynamicFrame : IAnimation
    {

        public Frame Frame;

        public DynamicFrame(Frame animation)
        {
            Frame = animation;
        }

        public float GetValue() => Frame.GetValue();
        public bool Completed => Frame.Completed;

    }
}
