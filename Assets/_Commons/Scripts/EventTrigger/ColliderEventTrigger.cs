using System;
using UnityEngine;

namespace Levels
{
    public abstract class ColliderEventTrigger<T> : MonoBehaviour where T : Collider
    {
        public event Action<Collider> Entered;
        public event Action<Collider> Staying;
        public event Action<Collider> Exited;

        protected T Trigger { get; private set; }

        public LayerMask IncludeLayers
        {
            get => Trigger.includeLayers;
            set => Trigger.includeLayers = value;
        }

        public LayerMask ExcludeLayers
        {
            get => Trigger.excludeLayers;
            set => Trigger.excludeLayers = value;
        }

        private void Awake()
        {
            Trigger = GetComponent<T>();
            Trigger.isTrigger = true;
        }

        private void OnDestroy()
        {
            Entered = null;
            Staying = null;
            Exited = null;
        }

        public void Enable() => Trigger.enabled = true;
        public void Disable() => Trigger.enabled = false;

        private void OnTriggerEnter(Collider other) => Entered?.Invoke(other);

        private void OnTriggerStay(Collider other) => Staying?.Invoke(other);

        private void OnTriggerExit(Collider other) => Exited?.Invoke(other);
    }
}
