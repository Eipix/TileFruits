using UnityEngine;
using UnityEngine.UI;

namespace Commons
{
    public class ScrollSliderConnector : MonoBehaviour
    {
        [SerializeField] private Slider _slider;
        [SerializeField] private ScrollRect _scrollView;

        private void Start()
        {
            _slider.value = 1 - _scrollView.verticalNormalizedPosition;
            _slider.onValueChanged.AddListener(OnSliderValueChanged);
            _scrollView.onValueChanged.AddListener(OnScrollViewChanged);
        }

        private void OnSliderValueChanged(float value)
        {
            _scrollView.verticalNormalizedPosition = 1f - value;
        }

        private void OnScrollViewChanged(Vector2 position)
        {
            _slider.value = 1f - position.y;
        }
    }
}
