using System;
using Commons.Systems.AudioManager;
using Commons.Systems.Save;
using Constants;
using Zenject;

namespace UI.Settings
{
    public class SettingsController : IInitializable, IDisposable
    {
        private readonly SettingsWindow _window;
        private readonly AudioManager _audioManager;
        private readonly ISaveSystem _saveSystem;

        public SettingsController(SettingsWindow window,
            AudioManager audioManager,
            ISaveSystem saveSystem)
        {
            _window = window;
            _audioManager = audioManager;
            _saveSystem = saveSystem;
        }

        public void Initialize()
        {
            _window.Opening += OnOpen;
            _window.Setup(OnMusicValueChanged, OnSoundValueChanged);
        }

        private void OnOpen()
        {
            _window.MusicToggleIsOn = _audioManager.MuteMusic;
            _window.SoundToggleIsOn = _audioManager.MuteSounds;
        }

        private void OnMusicValueChanged(bool isOn)
        {
            _saveSystem.Save(SaveKeys.MuteMusicBool, isOn);
            _audioManager.MuteMusic = isOn;
        }
        
        private void OnSoundValueChanged(bool isOn)
        {
            _saveSystem.Save(SaveKeys.MuteSoundBool, isOn);
            _audioManager.MuteSounds = isOn;
        }

        public void Dispose()
        {
            _window.Opening -= OnOpen;
        }
    }
}
