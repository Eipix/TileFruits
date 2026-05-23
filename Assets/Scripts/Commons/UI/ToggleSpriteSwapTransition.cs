using UnityEngine;
using UnityEngine.UI;

namespace Commons.UI
{
    [RequireComponent(typeof(Toggle))]
    public class ToggleSpriteSwapTransition : MonoBehaviour
    {
        [SerializeField] private Image _targetImage;
        
        [SerializeField] private Sprite _onIcon;
        [SerializeField] private Sprite _offIcon;
        
        private Toggle _toggle;

        private void Awake() => _toggle = GetComponent<Toggle>();

        private void OnEnable()
        {
            OnValueChanged(_toggle.isOn);
            _toggle.onValueChanged.AddListener(OnValueChanged);
        }

        private void OnDisable() => _toggle.onValueChanged.RemoveListener(OnValueChanged);

        private void OnValueChanged(bool isOn)
            => _targetImage.sprite = isOn
                ? _onIcon
                : _offIcon;
    }
}
