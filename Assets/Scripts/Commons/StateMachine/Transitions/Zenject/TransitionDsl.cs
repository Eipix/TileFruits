using System.Collections.Generic;
using Zenject;

namespace Commons.StateMachine.Transitions.Zenject
{
    public class TransitionDsl
    {
        public readonly DiContainer container;
        private readonly List<Transition> _items = new();

        public TransitionDsl(DiContainer container)
        {
            this.container = container;
        }

        public TransitionRule<TState> GoTo<TState>() where TState : State =>
            new(this, container.Resolve<TState>());

        public Transition[] Build() => _items.ToArray();

        public void Add(Transition t) => _items.Add(t);
    }
}
