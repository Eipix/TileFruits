using System.Collections.Generic;
using System.Linq;
using Commons.Extensions;
using DG.Tweening;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.Events;

namespace Entities.States
{
    public class MaskEffect : MonoBehaviour
    {
        private const string InvalidMaterialMessage = "All materials in the renderers must have the _OpacityThreshold property.";
        private static readonly int MaskThreshold = Shader.PropertyToID("_OpacityThreshold");

        [SerializeField, ValidateInput(nameof(HasMaskProperty), InvalidMaterialMessage)]
        private List<Renderer> _maskRenderers = new List<Renderer>();

        [SerializeField] private Ease _ease = Ease.OutCirc;
        [SerializeField] private float _duration = 1f;

        [field: SerializeField] public UnityEvent Masking { get; private set; }

        private MaterialPropertyBlock _propertyBlock;
        private Tween _masking;

        private float _currentValue;

        public bool IsActive => _masking.IsActive();

        private bool HasMaskProperty(List<Renderer> renderers)
        {
            if (renderers == null || renderers.Count == 0)
            {
                return true;
            }

            foreach (var renderer in renderers)
            {
                if (renderer == null || renderer.sharedMaterial == null)
                {
                    return false;
                }

                if (!renderer.sharedMaterial.HasProperty(MaskThreshold))
                {
                    return false;
                }
            }

            return true;
        }

        [Button]
        private void AutoCollectFromChildrens()
        {
            _maskRenderers.Clear();
            var renderers = GetComponentsInChildren<Renderer>();
            foreach (var renderer in renderers)
            {
                if (renderer.sharedMaterial != null && renderer.sharedMaterial.HasProperty(MaskThreshold))
                    _maskRenderers.Add(renderer);
            }
        }

        private void Awake()
        {
            _propertyBlock = new MaterialPropertyBlock();

            for (int i = _maskRenderers.Count - 1; i >= 0; i--)
            {
                if (_maskRenderers[i] == null)
                {
                    _maskRenderers.RemoveAt(i);
                    continue;
                }

                _maskRenderers[i].GetPropertyBlock(_propertyBlock);
            }
        }

        [Button(enabledMode: EButtonEnableMode.Playmode)]
        public Tween Mask()
        {
            Masking?.Invoke();
            return SetMask(1f, _duration);
        }

        [Button(enabledMode: EButtonEnableMode.Playmode)]
        public Tween Unmask() => SetMask(0f, _duration);

        public Tween SetMask(float value, float duration)
        {
            value = Mathf.Clamp(value, 0f, 1f);

            if (IsActive)
                _masking?.Kill();

            _masking = DOTween.To(() => _currentValue, SetValue, value, duration)
                .SetEase(_ease);

            return _masking;
        }

        public void Kill() => _masking?.Kill();

        public void SetValue(float value)
        {
            _currentValue = value;

            foreach (var renderer in _maskRenderers)
            {
                if (renderer == null)
                    continue;

                renderer.GetPropertyBlock(_propertyBlock);
                _propertyBlock.SetFloat(MaskThreshold, value);
                renderer.SetPropertyBlock(_propertyBlock);
            }
        }
    }
}
