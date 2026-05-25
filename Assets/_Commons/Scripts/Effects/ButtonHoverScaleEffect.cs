using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Commons.Effects
{
    public class ButtonHoverScale : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        [Header("Settings")]
        [SerializeField] private float scaleFactor = 1.1f;

        [SerializeField] private float duration = 0.15f;
        [SerializeField] private Ease easeType = Ease.OutQuad;

        private Vector3 _originalScale;

        private void Awake()
        {
            _originalScale = transform.localScale;
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            transform.DOKill();

            transform.DOScale(_originalScale * scaleFactor, duration)
                .SetEase(easeType)
                .SetUpdate(true);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            transform.DOKill();

            transform.DOScale(_originalScale, duration)
                .SetEase(easeType)
                .SetUpdate(true);
        }

        private void OnDisable()
        {
            transform.DOKill();
            transform.localScale = _originalScale;
        }
    }
}
