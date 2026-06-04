using _Commons.Scripts.Effects.Shakers;
using Commons.Extensions;
using DG.Tweening;
using NaughtyAttributes;
using UnityEngine;

namespace Commons.TweenEffects
{
    public class Shaker : MonoBehaviour
    {
        [SerializeField] private Transform _transform;
        [SerializeField] private ShakerConfig _config;

        private Tween _shaking;

        [Button(enabledMode: EButtonEnableMode.Playmode)]
        public Tween Shake()
        {
            _shaking.CompleteIfActive(true);
            _shaking = _transform.DOShakeRotation(_config.Duration,
                _config.Strength,
                _config.Vibrato,
                _config.Randomness,
                _config.FadeOut,
                _config.RandomnessMode);

            return _shaking;
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
    }
}
