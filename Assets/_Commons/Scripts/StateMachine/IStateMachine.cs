namespace Commons.StateMachine
{
    public interface IStateMachine<TState>
    {
        public TState Current { get; }

        public void ChangeState(TState newState);
    }
}
