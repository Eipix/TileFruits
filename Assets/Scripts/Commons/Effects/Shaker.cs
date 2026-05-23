using Commons.Extensions;
using DG.Tweening;
using NaughtyAttributes;
using UnityEngine;

namespace Commons.TweenEffects
{
    public class Shaker : MonoBehaviour
    {
        [SerializeField] private Transform _transform;
        [SerializeField] private float _duration = 1f;
        [SerializeField] private Vector3 _strength = new(1f, 1f, 3f);
        [SerializeField] private int _vibrato = 20;
        [SerializeField] private float _randomness = 40f;
        [SerializeField] private bool _fadeOut = true;
        [SerializeField] private ShakeRandomnessMode _randomnessMode = ShakeRandomnessMode.Full;

        private Tween _shaking;

        public bool IsActive => _shaking.NotNullAndActive();

        [Button(enabledMode: EButtonEnableMode.Playmode)]
        public void Shake()
        {
            _shaking.CompleteIfActive(true);
            _shaking = _transform.DOShakeRotation(_duration, _strength, _vibrato, _randomness, _fadeOut, _randomnessMode);
        }

        public Tween Shake(
            float duration,
            Vector3 strength,
            int vibrato = 10,
            float randomness = 90f,
            bool fadeOut = true,
            ShakeRandomnessMode randomnessMode = ShakeRandomnessMode.Full)
        {
            _shaking.CompleteIfActive(true);
            _shaking = _transform.DOShakeRotation(duration, strength, vibrato, randomness, fadeOut, randomnessMode);
            return _shaking;
        }

        public void Complete()
        {
            if (IsActive)
                _shaking.Complete(true);
        }
    }
}
