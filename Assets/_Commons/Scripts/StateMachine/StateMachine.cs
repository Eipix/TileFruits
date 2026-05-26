using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;
using Zenject;

namespace Commons.StateMachine
{
    public class StateMachine : MonoBehaviour, IStateMachine<State>
    {
        private readonly Dictionary<Type, State> _statesByType = new();

        [SerializeField] private State _defaultState;

        public event Action<State> StateChanged;

        public IReadOnlyDictionary<Type, State> StatesByType => _statesByType;

        public State Current { get; private set; }

        [Inject]
        public void Constructor(State[] states)
        {
            foreach (var state in states)
            {
                bool enable = state == _defaultState;
                TryAddState(state, enable);
            }

            Current = _defaultState;
        }

        protected virtual void Update()
        {
            if (Current.CanTransition(out State nextState))
                ChangeState(nextState);
        }

        protected bool TryAddState(State state, bool enabled = false)
        {
            var type = state.GetType();

            if (_statesByType.ContainsKey(type))
                return false;

            state.enabled = enabled;
            _statesByType.Add(type, state);
            return true;
        }

        public bool TryGetState<T>(out T targetState) where T : State
        {
            if (_statesByType.TryGetValue(typeof(T), out var state))
            {
                targetState = (T)state;
                return true;
            }
            targetState = null;
            return false;
        }

        [CanBeNull]
        public T GetState<T>() where T : State
        {
            bool result = TryGetState(out T targetState);

            if (result)
            {
                return targetState;
            }

            return null;
        }

        public void ChangeState(State nextState)
        {
            var type = nextState.GetType();

            if (Current.GetType() == type)
                return;

            if (_statesByType.TryGetValue(type, out State state) is false)
                throw new InvalidOperationException($"The FSM does not contain a state {type.Name}");

            Current.enabled = false;
            Current = state;
            Current.enabled = true;

            StateChanged?.Invoke(Current);
        }
    }
}
