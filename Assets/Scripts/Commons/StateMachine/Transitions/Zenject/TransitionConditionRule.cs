using Commons.Specifications;

namespace Commons.StateMachine.Transitions.Zenject
{
    public class TransitionConditionRule<TState> where TState : State
    {
        private readonly TransitionDsl _owner;
        private readonly State _state;
        private ISpecification _spec;

        public TransitionConditionRule(TransitionDsl owner, State state, ISpecification spec)
        {
            _owner = owner;
            _state = state;
            _spec = spec;
        }

        public TransitionConditionRule<TState> And<TSpec>() where TSpec : ISpecification
        {
            _spec = _spec.And(_owner.container.Resolve<TSpec>());
            return this;
        }

        public TransitionConditionRule<TState> Or<TSpec>() where TSpec : ISpecification
        {
            _spec = _spec.Or(_owner.container.Resolve<TSpec>());
            return this;
        }

        public TransitionConditionRule<TState> AndNot<TSpec>() where TSpec : ISpecification
        {
            _spec = _spec.And(_owner.container.Resolve<TSpec>().Not());
            return this;
        }

        public TransitionConditionRule<TState> OrNot<TSpec>() where TSpec : ISpecification
        {
            _spec = _spec.Or(_owner.container.Resolve<TSpec>().Not());
            return this;
        }

        public TransitionDsl Done()
        {
            _owner.Add(new TransitionWithSpecification(_state, _spec));
            return _owner;
        }
    }
}
