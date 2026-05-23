using Commons;
using Commons.Systems.AudioManager;
using Commons.Systems.Save;
using Constants;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace UI
{
    public class SettingsWindow : Window
    {
        [SerializeField] private Button _closeButton;
        [SerializeField] private Toggle _musicToggle;
        [SerializeField] private Toggle _soundToggle;

        private ISaveSystem _saveSystem;
        private AudioManager _audioManager;
        
        [Inject]
        private void Construct(ISaveSystem saveSystem, AudioManager audioManager)
        {
            _saveSystem = saveSystem;
            _audioManager = audioManager;
        }

        protected override void OnOpen()
        {
            _musicToggle.isOn = _saveSystem.Load(SaveKeys.MuteMusic, false);
            _soundToggle.isOn = _saveSystem.Load(SaveKeys.MuteSound, false);
            
            _closeButton.onClick.AddListener(Close);
            _musicToggle.onValueChanged.AddListener(OnMusicValueChanged);
            _soundToggle.onValueChanged.AddListener(OnSoundValueChanged);
        }

        protected override void OnClose()
        {
            _closeButton.onClick.RemoveListener(Close);
            _musicToggle.onValueChanged.RemoveListener(OnMusicValueChanged);
            _soundToggle.onValueChanged.RemoveListener(OnSoundValueChanged);
        }

        private void OnMusicValueChanged(bool isOn)
        {
            _saveSystem.Save(SaveKeys.MuteMusic, isOn);
            _audioManager.MuteMusic = isOn;
        }
        
        private void OnSoundValueChanged(bool isOn)
        {
            _saveSystem.Save(SaveKeys.MuteSound, isOn);
            _audioManager.MuteSounds = isOn;
        }
    }
}
