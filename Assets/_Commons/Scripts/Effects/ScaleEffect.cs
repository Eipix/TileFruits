using System;
using Commons.Extensions;
using DG.Tweening;
using Effects;
using NaughtyAttributes;
using UnityEngine;

namespace Commons.Effects
{
    public class ScaleEffect : UIAnimation
    {
        [SerializeField] private Transform _target;
        [SerializeField] private ScaleSettings _showSettings;
        [SerializeField] private ScaleSettings _hideSettings;

        private Tween _scaling;
        private Vector3 _initialScale;

        private void Awake() => _initialScale = _target.localScale;

        public override Tween Show()
        {
            _scaling.CompleteIfActive();
            _scaling = ChangeScale(_initialScale, _showSettings);

            return _scaling;
        }

        public override Tween Close()
        {
            _scaling.CompleteIfActive();
            _scaling = ChangeScale(Vector2.zero, _hideSettings);

            return _scaling;
        }

        private Tween ChangeScale(Vector2 newScale, ScaleSettings settings)
        {
            var tween = _target.DOScale(newScale, settings.Duration)
                .SetLink(gameObject);

            var ease = settings.Ease;

            return ease is Ease.INTERNAL_Custom
                ? tween.SetEase(settings.CustomCurve) 
                : tween.SetEase(ease, settings.OverShoot);
        }

        [Serializable]
        public class ScaleSettings
        {
            [field: SerializeField, ShowIf(nameof(Ease), Ease.INTERNAL_Custom), AllowNesting]
            public AnimationCurve CustomCurve { get; private set; }
            
            [field: SerializeField, AllowNesting, OnValueChanged(nameof(OnValueChanged))]
            public Ease Ease { get; private set; } = Ease.OutBounce;
            [field: SerializeField, Min(0f)] public float Duration { get; private set; } = 1f;
            
            [field: SerializeField, Min(0f), HideIf(nameof(Ease), Ease.INTERNAL_Custom), AllowNesting]
            public float OverShoot { get; private set; } = 1;

            private void OnValueChanged() => CustomCurve = Ease.ToAnimationCurve();
        }
    }
}
