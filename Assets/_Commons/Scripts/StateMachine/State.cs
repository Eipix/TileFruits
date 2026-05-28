using Commons.StateMachine.Transitions;
using UnityEngine;

namespace Commons.StateMachine
{
    [RequireComponent(typeof(global::Commons.StateMachine.StateMachine))]
    public abstract class State : MonoBehaviour
    {
        private Transition[] _transitions;

        public Transition[] Transitions => _transitions;

        public void Init(Transition[] transitions)
        {
            _transitions = transitions;
        }

        public bool CanTransition(out State state)
        {
            if (_transitions == null)
            {
                state = null;
                return false;
            }

            foreach (var transition in _transitions)
            {
                if (transition.CanTransition() && transition.NextState != this)
                {
                    state = transition.NextState;
                    return true;
                }
            }

            state = null;
            return false;
        }
    }
}
