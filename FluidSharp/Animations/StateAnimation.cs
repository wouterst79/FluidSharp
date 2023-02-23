using System.Collections.Generic;

namespace FluidSharp.Animations
{
    public class StateAnimation : Animation
    {

        public List<State> States { get; set; } = new List<State>();

        public State? CurrentState { get; private set; }

        public State Add(State state)
        {
            States.Add(state);
            return state;
        }

        public void StartState(State state)
        {
            state.Animation.Start();
            CurrentState = state;
            state.Started = true;
        }

    }
}
