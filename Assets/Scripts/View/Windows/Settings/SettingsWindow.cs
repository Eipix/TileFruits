using Commons;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace UI
{
    public class SettingsWindow : Window
    {
        [SerializeField] private Button _closeButton;
        [SerializeField] private Toggle _musicToggle;
        [SerializeField] private Toggle _soundToggle;
        
        private UnityAction<bool> _onMusicValueChanged;
        private UnityAction<bool> _onSoundValueChanged;

        public bool MusicToggleIsOn
        {
            get => _musicToggle.isOn;
            set => _musicToggle.isOn = value;
        }
        
        public bool SoundToggleIsOn
        {
            get => _soundToggle.isOn;
            set => _soundToggle.isOn = value;
        }

        public void Setup(UnityAction<bool> onMusicValueChanged, UnityAction<bool> onSoundValueChanged)
        {
            _onMusicValueChanged = onMusicValueChanged;
            _onSoundValueChanged = onSoundValueChanged;
        }

        protected override void OnOpen()
        {
            _closeButton.onClick.AddListener(Close);
            _musicToggle.onValueChanged.AddListener(_onMusicValueChanged);
            _soundToggle.onValueChanged.AddListener(_onSoundValueChanged);
        }

        protected override void OnClose()
        {
            _closeButton.onClick.RemoveListener(Close);
            _musicToggle.onValueChanged.RemoveListener(_onMusicValueChanged);
            _soundToggle.onValueChanged.RemoveListener(_onSoundValueChanged);
        }
    }
}
