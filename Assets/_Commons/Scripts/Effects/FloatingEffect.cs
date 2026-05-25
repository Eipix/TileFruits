using System;
using UnityEngine;
using DG.Tweening;

namespace Effects
{
    public class FloatingEffect : MonoBehaviour
    {
        [SerializeField] private bool _useIndependentUpdate = true;
        [SerializeField] private float _radius = 10f;
        [SerializeField] private float _duration = 4f;
        [SerializeField] private Vector3 _planeNormal = Vector3.forward;

        private Transform _transform;
        private RectTransform _rectTransform;
        private Action<Vector3> _changePosition;

        private Vector3 _centerLocalPosition;

        private void Awake()
        {
            _transform = transform;
            _rectTransform = _transform as RectTransform;

            if (_rectTransform is null)
            {
                _centerLocalPosition = _transform.localPosition;
                _changePosition = position => _transform.localPosition = position;
            }
            else
            {
                _centerLocalPosition = _rectTransform.anchoredPosition;
                _changePosition = position => _rectTransform.anchoredPosition = position;
            }
        }

        private void Start()
        {
            DOTween.To(
                    () => 0f,
                    angleFraction =>
                    {
                        float angle = angleFraction * Mathf.PI * 2f;
                        Vector3 offset = GetCircularOffset(angle, _radius, _planeNormal);
                        _changePosition.Invoke(_centerLocalPosition + offset);
                    },
                    1f,
                    _duration
                )
                .SetLoops(-1, LoopType.Restart)
                .SetEase(Ease.Linear)
                .SetUpdate(_useIndependentUpdate);
        }

        private void OnDestroy() => _transform?.DOKill();

        private static Vector3 GetCircularOffset(float angle, float radius, Vector3 normal)
        {
            normal = normal.normalized;

            Vector3 tangent = Mathf.Abs(Vector3.Dot(normal, Vector3.up)) < 0.99f
                ? Vector3.Cross(normal, Vector3.up).normalized
                : Vector3.Cross(normal, Vector3.right).normalized;

            Vector3 bitangent = Vector3.Cross(normal, tangent).normalized;
            return radius * (Mathf.Cos(angle) * tangent + Mathf.Sin(angle) * bitangent);
        }
    }
}
