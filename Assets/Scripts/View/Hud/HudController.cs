using System;
using Gameplay.Levels;
using Zenject;

namespace UI
{
    public class HudController : IInitializable, IDisposable
    {
        private readonly Hud _hud;
        private readonly SettingsWindow _settingsWindow;
        private readonly LevelManager _levelManager;

        public HudController(Hud hud, SettingsWindow settingsWindow, LevelManager levelManager)
        {
            _hud = hud;
            _settingsWindow = settingsWindow;
            _levelManager = levelManager;
        }

        public void Initialize()
        {
            _hud.Setup(_settingsWindow.Open,
                _settingsWindow.Open);

            _levelManager.LevelStarted += SetLevelNumber;
        }

        private void SetLevelNumber() => _hud.SetLevel(_levelManager.LevelIndex + 1);

        public void Dispose()
        {
            if (_levelManager != null)
                _levelManager.LevelStarted -= SetLevelNumber;
        }
    }
}
