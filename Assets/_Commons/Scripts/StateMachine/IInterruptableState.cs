namespace Commons.StateMachine
{
    public interface IInterruptableState
    {
        public bool CanBeInterrupted();
    }
}
