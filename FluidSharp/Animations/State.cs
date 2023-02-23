using System;

namespace FluidSharp.Animations
{
    public class State
    {

        public string Name { get; set; }

        public Animation.Coordinated Animation = new Animation.Coordinated(DateTime.Now);

        public State(string name) => Name = name;

        public bool Started { get; set; }
        public bool Completed => Started && Animation.Completed;

        public Animation.Coordinated? AnimationIfStarted => Started ? Animation : null;

        public void Reset()
        {
            Started = false;
        }

    }
}
