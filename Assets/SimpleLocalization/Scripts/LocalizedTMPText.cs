using NaughtyAttributes;
using TMPro;
using UnityEngine;

namespace Assets.SimpleLocalization.Scripts
{
    [RequireComponent(typeof(TextMeshProUGUI))]
    public class LocalizedTMPText : MonoBehaviour
    {
        [field: SerializeField, Dropdown(nameof(GetKeys))]
        public string Key { get; private set; }

        private TextMeshProUGUI _text;
        
        private void Awake() => _text = GetComponent<TextMeshProUGUI>();

        private string[] GetKeys() => LocalizationManager.GetKeys();

        public void Start() => Localize();

        private void OnEnable() => LocalizationManager.OnLocalizationChanged += Localize;
        private void OnDisable() => LocalizationManager.OnLocalizationChanged -= Localize;

        private void Localize()
        {
            _text.text = LocalizationManager.Localize(Key);
        }
    }
}
