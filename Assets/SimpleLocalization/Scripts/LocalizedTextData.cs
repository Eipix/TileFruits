using NaughtyAttributes;
using UnityEngine;

namespace Assets.SimpleLocalization.Scripts
{
    [CreateAssetMenu(fileName = "Localization/LocalizedTextData")]
    public class LocalizedTextData : ScriptableObject
    {
        [field: SerializeField, Dropdown(nameof(GetKeys))]
        public string Key { get; private set; }

        public string Text => LocalizationManager.Localize(Key);
        
        private string[] GetKeys() => LocalizationManager.GetKeys();
    }
}
