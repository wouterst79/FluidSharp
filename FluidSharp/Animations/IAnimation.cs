using System;
using System.Collections.Generic;
using System.Text;

namespace FluidSharp.Animations
{
    public interface IAnimation
    {

        bool Completed { get; }

        public float GetValue();

    }
}
