using System;
using System.Collections.Generic;
using DG.Tweening;
using NaughtyAttributes;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Effects
{
    [DisallowMultipleComponent]
    public class DissolveEffect : UIAnimation
    {
        private static readonly int DissolveAmount = Shader.PropertyToID("_DissolveAmount");
        private static readonly int EdgeWidth = Shader.PropertyToID("_EdgeWidth");
        private static readonly int EdgeColorOuter = Shader.PropertyToID("_EdgeColorOuter");
        private static readonly int EdgeColorInner = Shader.PropertyToID("_EdgeColorInner");

        private readonly List<Graphic> _graphics = new();

        [SerializeField] private bool _ignoreTimeScale = true;
        [SerializeField, Min(0f)] private float _duration = 0.5f;

        [SerializeField, Range(0f, 1f), OnValueChanged(nameof(OnAmountChanged))]
        private float _propertyValue;

        [SerializeField, Range(0.0f, 0.3f), OnValueChanged(nameof(OnEdgeWidthChanged))]
        private float _edgeWidth = 0.3f;

        [SerializeField, ColorUsage(true, true), OnValueChanged(nameof(OnOuterChanged))]
        private Color _outer = Color.blue;

        [SerializeField, ColorUsage(true, true), OnValueChanged(nameof(OnInnerChanged))]
        private Color _inner = Color.blue;

        private Tween _tween;

        private void OnAmountChanged() => SetDissolve(_propertyValue);
        private void OnEdgeWidthChanged() => SetFloat(EdgeWidth, _edgeWidth);

        private void OnOuterChanged() => SetColor(EdgeColorOuter, _outer);
        private void OnInnerChanged() => SetColor(EdgeColorInner, _inner);

        private void Start() => InitializeGraphics();

        [Button]
        private void InitializeGraphics()
        {
            Graphic[] graphics = GetComponentsInChildren<Graphic>(true);

            foreach (Graphic graphic in graphics)
            {
                if (graphic.material == null)
                    continue;

                if (graphic.material.HasFloat(DissolveAmount)
                    || graphic is TextMeshProUGUI tmpText && tmpText.fontMaterial.HasFloat(DissolveAmount))
                    _graphics.Add(graphic);

                if (graphic is not TextMeshProUGUI)
                    graphic.material = new(graphic.material);
            }

            OnAmountChanged();
            OnEdgeWidthChanged();
            OnOuterChanged();
            OnInnerChanged();
        }

        [Button(enabledMode: EButtonEnableMode.Playmode)]
        public void EnableTest() => DODissolve(0f);

        [Button(enabledMode: EButtonEnableMode.Playmode)]
        public void DisableTest() => DODissolve(1f);

        public override Tween Show()
        {
            SetDissolve(1f);
            return DODissolve(0f);
        }

        public override Tween Close()
        {
            SetDissolve(0f);
            return DODissolve(1f);
        }

        private Tween DODissolve(float value)
        {
            _tween?.Kill();

            _tween = DOTween.To(Getter, SetDissolve, value, _duration)
                .SetUpdate(_ignoreTimeScale);

            return _tween;

            float Getter() => _propertyValue;
        }

        private void SetDissolve(float value)
        {
            _propertyValue = value;
            SetFloat(DissolveAmount, _propertyValue);
        }

        private void SetFloat(int nameID, float value)
        {
            ForEachMaterial(mat =>  mat.SetFloat(nameID, value));
        }

        private void SetColor(int nameID, Color value)
        {
            ForEachMaterial(mat =>  mat.SetColor(nameID, value));
        }

        private void ForEachMaterial(Action<Material> action)
        {
            foreach (Graphic graphic in _graphics)
            {
                if (graphic is TextMeshProUGUI tmpText)
                    action.Invoke(tmpText.fontMaterial);
                else
                    action.Invoke(graphic.material);

                graphic.SetMaterialDirty();
            }
        }

        private void OnDestroy()
        {
            foreach (Graphic graphic in _graphics)
            {
                if (graphic == null)
                    continue;

                if (graphic is not TextMeshProUGUI)
                    Destroy(graphic.material);
            }

            _graphics.Clear();
        }
    }
}
