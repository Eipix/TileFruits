using Commons.Specifications;

namespace Commons.StateMachine.Transitions
{
    public class TransitionWithSpecification : Transition
    {
        protected readonly ISpecification _specification;

        public TransitionWithSpecification(State nextState, ISpecification specification) : base(nextState)
        {
            _specification = specification;
        }

        public override bool CanTransition() => _specification.IsSatisfied();
    }
}
