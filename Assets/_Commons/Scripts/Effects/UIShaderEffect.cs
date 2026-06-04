using System;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using Effects;
using NaughtyAttributes;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace _Commons.Scripts.Effects
{
    public class UIShaderEffect : UIAnimation
    {
        [SerializeField] private List<Graphic> _graphics = new();

        [SerializeField] private string _propertyName;
        
        [SerializeField] private bool _ignoreTimeScale = true;
        [SerializeField, Min(0f)] private float _duration = 0.5f;
        [SerializeField] private Ease _ease = Ease.Linear;
        
        [SerializeField, OnValueChanged(nameof(OnAmountChanged))]
        private float _propertyValue;

        [SerializeField] private MinMax _minMaxRange;

        private Tween _tween;
        private int _propertyID;

        private void OnAmountChanged() => SetFloat(_propertyValue);

        [Button]
        private void Collect()
        {
            _graphics.Clear();
            _propertyID = Shader.PropertyToID(_propertyName);
            Graphic[] graphics = GetComponentsInChildren<Graphic>(true);

            foreach (Graphic graphic in graphics)
            {
                if (graphic.material == null)
                    continue;

                if (graphic.material.HasFloat(_propertyID)
                    || graphic is TextMeshProUGUI tmpText && tmpText.fontMaterial.HasFloat(_propertyID))
                    _graphics.Add(graphic);

                if (graphic is not TextMeshProUGUI)
                    graphic.material = new(graphic.material);
            }

            OnAmountChanged();
        }

        [Button(enabledMode: EButtonEnableMode.Playmode)]
        public override Tween Show() => DOFloat(_minMaxRange.Max);

        [Button(enabledMode: EButtonEnableMode.Playmode)]
        public override Tween Close() => DOFloat(_minMaxRange.Min);

        private Tween DOFloat(float value)
        {
            _tween?.Kill();

            _tween = DOTween.To(() => _propertyValue, SetFloat, value, _duration)
                .SetEase(_ease)
                .SetUpdate(_ignoreTimeScale);

            return _tween;
        }

        private void SetFloat(float value)
        {
            _propertyValue = value;
            ForEachMaterial(mat =>  mat.SetFloat(_propertyID, _propertyValue));
        }

        protected void ForEachMaterial(Action<Material> action)
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

        [Serializable]
        public struct MinMax
        {
            public float Min;
            public float Max;
        }
    }
}
