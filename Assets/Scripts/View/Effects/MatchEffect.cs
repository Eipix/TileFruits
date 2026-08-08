using DG.Tweening;
using UnityEngine;
using Zenject;

namespace View.Effects
{
    [RequireComponent(typeof(ParticleSystem))]
    public class MatchEffect : MonoBehaviour
    {
        private ParticleSystem _matchEffect;

        private void Awake() => _matchEffect = GetComponent<ParticleSystem>();

        public class Pool : MonoMemoryPool<Vector2, MatchEffect>
        {
            protected override void Reinitialize(Vector2 position, MatchEffect item)
            {
                base.Reinitialize(position, item);

                item.transform.position = position;
                
                DOVirtual.DelayedCall(item._matchEffect.main.duration, 
                    () => Despawn(item),
                    false);
            }

            protected override void OnDespawned(MatchEffect item)
            {
                base.OnDespawned(item);
                item._matchEffect.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
            }
        }
    }
}
