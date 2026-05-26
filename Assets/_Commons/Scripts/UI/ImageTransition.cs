using Commons.Extensions;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Commons.UI
{
    public class ImageTransition : MonoBehaviour,
        IPointerEnterHandler, IPointerExitHandler,
        IPointerDownHandler, IPointerUpHandler
    {
        [SerializeField] private Image _sourceImage;

        [Space, Header("Transition Sprites")]
        [SerializeField] private float _upAlpha;
        [SerializeField] private float _downAlpha;
        [SerializeField] private float _enterAlpha;
        [SerializeField] private float _exitAlpha;

        private bool _isPointerDown;
        private bool _isPointerEntered;

        private float Alpha
        {
            get => _sourceImage.color.a;
            set => _sourceImage.Fade(value);
        }

        private void Awake() => Alpha = _exitAlpha;

        public void OnPointerEnter(PointerEventData eventData)
        {
            Alpha = _enterAlpha;
            _isPointerEntered = true;
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            if (_isPointerDown)
            {
                _isPointerEntered = false;
                return;
            }

            Alpha = _exitAlpha;
            _isPointerEntered = false;
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            Alpha = _downAlpha;
            _isPointerDown = true;
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            if (_isPointerEntered is false)
            {
                Alpha = _exitAlpha;
                _isPointerDown = false;
                return;
            }

            Alpha = _upAlpha;
            _isPointerDown = false;
        }
    }
}
