using Assets.SimpleLocalization.Scripts;
using NaughtyAttributes;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace UI
{
    public class Hud : MonoBehaviour
    {
        [SerializeField, Expandable] private LocalizedTextData _localizedLevel;
        [SerializeField] private TMP_Text _levelNumber;
        [SerializeField] private Button _settingsButton;
        [SerializeField] private Button _collectionButton;

        private UnityAction _onSettingsClick;
        private UnityAction _onCollectionClick;

        private void OnDisable()
        {
            _settingsButton.onClick.RemoveListener(_onSettingsClick);
            _collectionButton.onClick.RemoveListener(_onCollectionClick);
        }

        public void Setup(UnityAction onSettingsClick, UnityAction onCollectionClick)
        {
            _onSettingsClick = onSettingsClick;
            _onCollectionClick = onCollectionClick;
            
            _settingsButton.onClick.AddListener(_onSettingsClick);
            _collectionButton.onClick.AddListener(_onCollectionClick);
        }

        public void SetLevel(int levelNumber) =>
            _levelNumber.SetText(_localizedLevel.Text, levelNumber);
    }
}
