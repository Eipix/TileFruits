namespace Commons.StateMachine.Transitions
{
    public abstract class Transition
    {
        public State NextState { get; }

        protected Transition(State nextState) => NextState = nextState;

        public abstract bool CanTransition();
    }
}
