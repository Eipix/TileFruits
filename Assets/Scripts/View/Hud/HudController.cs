using System;
using _Commons.Scripts.UI;
using Gameplay.Levels;
using View.Windows.Collection;
using Zenject;

namespace UI
{
    public class HudController : IInitializable, IDisposable
    {
        private readonly Hud _hud;
        private readonly UIManager _uiManager;
        private readonly LevelManager _levelManager;

        public HudController(Hud hud, UIManager uiManager, LevelManager levelManager)
        {
            _hud = hud;
            _uiManager = uiManager;
            _levelManager = levelManager;
        }

        public void Initialize()
        {
            var settingsWindow = _uiManager.GetWindow<SettingsWindow>();
            var collectionWindow = _uiManager.GetWindow<CollectionWindow>();
            
            _hud.Setup(settingsWindow.Open, collectionWindow.Open);

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
