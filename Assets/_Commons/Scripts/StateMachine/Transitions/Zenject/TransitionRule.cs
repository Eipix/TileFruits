using Commons.Specifications;

namespace Commons.StateMachine.Transitions.Zenject
{
    public class TransitionRule<TState> where TState : State
    {
        private readonly TransitionDsl _owner;
        private readonly State _state;

        public TransitionRule(TransitionDsl owner, State state)
        {
            _owner = owner;
            _state = state;
        }

        public TransitionConditionRule<TState> When<TSpec>() where TSpec : ISpecification
        {
            var spec = _owner.container.Resolve<TSpec>();
            return new TransitionConditionRule<TState>(_owner, _state, spec);
        }

        public TransitionConditionRule<TState> WhenNot<TSpec>() where TSpec : ISpecification
        {
            var spec = _owner.container.Resolve<TSpec>().Not();
            return new TransitionConditionRule<TState>(_owner, _state, spec);
        }
    }
}
