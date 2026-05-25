using Commons.Extensions;
using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using NaughtyAttributes;
using UnityEngine;

namespace Effects
{
    [RequireComponent(typeof(Renderer))]
    public class FlashEffect : MonoBehaviour
    {
        private static readonly int EmissionColorId = Shader.PropertyToID("_EmissionColor");

        [SerializeField] private Ease _ease;
        [SerializeField] private Color _baseColor = Color.red;
        [SerializeField, Range(-10f, 10f)] private float _startIntensity;
        [SerializeField, Range(-10f, 10f)] private float _endIntensity = 10f;
        [SerializeField, Min(0f)] private float _duration = 0.3f;

        private Renderer _renderer;
        private MaterialPropertyBlock _propertyBlock;

        private Sequence _flashing;

        private void Awake()
        {
            _renderer = GetComponent<Renderer>();
            _propertyBlock = new MaterialPropertyBlock();
        }

        [Button(enabledMode: EButtonEnableMode.Playmode)]
        private void StartFlash()
        {
            _flashing.CompleteIfActive(true);
            float tweenDuration = _duration / 2f;

            _flashing = DOTween.Sequence()
                .OnStart(() => SetIntensity(_startIntensity))
                .Append(DOEmission(_endIntensity, tweenDuration))
                .Append(DOEmission(_startIntensity, tweenDuration))
                .SetEase(_ease);
        }

        private TweenerCore<float, float, FloatOptions> DOEmission(float endValue, float duration)
        {
            return DOTween.To(GetEmissionIntensity, SetIntensity, endValue, duration);
        }

        private void SetIntensity(float intensity)
        {
            _renderer.GetPropertyBlock(_propertyBlock);
            Color emissionColor = _baseColor * intensity;
            _propertyBlock.SetColor(EmissionColorId, emissionColor);
            _renderer.SetPropertyBlock(_propertyBlock);
        }

        public Color GetEmissionColor()
        {
            _renderer.GetPropertyBlock(_propertyBlock);
            Color blockedColor = _propertyBlock.GetColor(EmissionColorId);

            if (blockedColor != Color.black)
            {
                return blockedColor;
            }
            return _renderer.sharedMaterial.GetColor(EmissionColorId);
        }

        public float GetEmissionIntensity()
        {
            Color emission = GetEmissionColor();
            return (emission.r + emission.g + emission.b) / 3f;
        }
    }
}
