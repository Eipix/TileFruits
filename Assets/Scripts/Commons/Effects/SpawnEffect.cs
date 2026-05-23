using Entities.States;
using UnityEngine;

namespace Effects
{
    [RequireComponent(typeof(MaskEffect))]
    public class SpawnEffect : MonoBehaviour
    {
        [SerializeField] private bool _playOnStart = true;

        private MaskEffect _maskEffect;
        private ParticleSystem _particleInstance;

        private void Awake() => _maskEffect = GetComponent<MaskEffect>();

        private void Start()
        {
            if(_playOnStart)
                StartEffect();
        }

        private void OnDisable() => _maskEffect.Kill();

        public void StartEffect()
        {
            _maskEffect.SetValue(1f);
            _maskEffect.Unmask();
        }
    }
}
