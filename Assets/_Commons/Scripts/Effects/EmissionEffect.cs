using System.Collections.Generic;
using System.Linq;
using Commons.Extensions;
using DG.Tweening;
using NaughtyAttributes;
using UnityEngine;

namespace Commons.Effects
{
    public class EmissionEffect : MonoBehaviour
    {
        private const string InvalidMaterialMessage = "All materials in the renderers must have the _EmissionColor property.";
        private static readonly int EmissionColorId = Shader.PropertyToID("_EmissionColor");

        [SerializeField, ValidateInput(nameof(HasEmissionProperty), InvalidMaterialMessage)]
        private List<Renderer> _renderers;

        [SerializeField, ColorUsage(true, true)]
        private Color _from = Color.black;

        [SerializeField, ColorUsage(true, true)]
        private Color _to = Color.white * 3f;

        [SerializeField, Min(0f)] private float _duration = 0.5f;
        [SerializeField, Min(0f)] private float _afterBlinkCooldown = 0.05f;
        [SerializeField, Min(0f)] private int _repeatCount = 2;

        private Sequence _blinking;
        private MaterialPropertyBlock _propertyBlock;

        private bool HasEmissionProperty(List<Renderer> renderers)
        {
            if(renderers == null)
                return false;

            return !renderers.Select(renderer => renderer.sharedMaterial)
                .Any(mat => mat.HasColor(EmissionColorId) is false);
        }

        [Button]
        private void AutoCollectFromChildrens()
        {
            var renderers = GetComponentsInChildren<Renderer>();

            foreach (var renderer in renderers)
            {
                if (renderer.sharedMaterial.HasFloat(EmissionColorId))
                    _renderers.Add(renderer);
            }
        }

        private void Awake()
        {
            _propertyBlock = new();

            foreach (var renderer in _renderers)
                renderer.GetPropertyBlock(_propertyBlock);
        }

        [Button(enabledMode: EButtonEnableMode.Playmode)]
        public Sequence StartBlinking()
        {
            _blinking.CompleteIfActive();

            _blinking = DOTween.Sequence()
                .SetLink(gameObject, LinkBehaviour.CompleteOnDisable);

            float stepDuration = _duration / 2;

            for (int i = 0; i <= _repeatCount; i++)
            {
                Blink(_blinking, _from, _to, stepDuration);
                Blink(_blinking, _to, _from, stepDuration);
            }

            return _blinking;

            void Blink(Sequence sequence, Color from, Color to, float duration)
            {
                sequence.Append(DOVirtual.Color(from, to, duration, SetEmission));
                sequence.AppendInterval(_afterBlinkCooldown);
            }
        }

        private void SetEmission(Color color)
        {
            foreach (var renderer in _renderers)
            {
                _propertyBlock.SetColor(EmissionColorId, color);
                renderer.SetPropertyBlock(_propertyBlock);
            }
        }
    }
}
