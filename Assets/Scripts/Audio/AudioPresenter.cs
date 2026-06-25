using System;
using _Commons.Scripts.UI;
using Commons.Systems.AudioManager;
using Gameplay;
using Gameplay.Levels;
using Gameplay.Tray;
using View.Windows.Collection;

namespace Audio
{
    public class AudioPresenter : IDisposable
    {
        private readonly AudioManager _audioManager;
        private readonly AudioConfig _audioConfig;
        private readonly LevelManager _levelManager;
        private readonly TileTray _tileTray;
        private readonly GameplayController _gameplayController;
        private readonly CollectionWindow _collectionWindow;

        public AudioPresenter(
            AudioManager audioManager,
            AudioConfig audioConfig,
            LevelManager levelManager,
            TileTray tileTray,
            GameplayController gameplayController,
            UIManager uiManager)

        {
            _audioManager = audioManager;
            _audioConfig = audioConfig;
            _levelManager = levelManager;
            _tileTray = tileTray;
            _gameplayController = gameplayController;
            _collectionWindow = uiManager.GetWindow<CollectionWindow>();
        }

        public void Initialize()
        {
            _levelManager.LevelStarted += PlayLevelStartedSound;
            _levelManager.LevelFinished += PlayLevelFinishedSound;
            
            _tileTray.MatchCleared += PlayMatchesSound;
            _tileTray.Added += PlayTileMoveToTraySound;
            _gameplayController.TileBlocked += PlayTileBlockedSound;
            _collectionWindow.TilePointerDown += PlayTileBlockedSound;
            
            PlayTheme();
        }

        public void Dispose()
        {
            _levelManager.LevelStarted -= PlayLevelStartedSound;
            _levelManager.LevelFinished -= PlayLevelFinishedSound;
            
            _tileTray.MatchCleared -= PlayMatchesSound;
            _tileTray.Added -= PlayTileMoveToTraySound;
            _gameplayController.TileBlocked -= PlayTileBlockedSound;
            _collectionWindow.TilePointerDown -= PlayTileBlockedSound;
            
            _audioManager.Stop();
        }

        private void PlayTileBlockedSound()
        {
            _audioManager.PlayOneShot(_audioConfig.TileBlocked);
        }

        private void PlayTileMoveToTraySound(TileConfig config, int index)
        {
            _audioManager.PlayOneShot(_audioConfig.TileMoveToTray);
        }

        private void PlayMatchesSound(TileConfig config)
        {
            _audioManager.PlayOneShot(_audioConfig.TileMatches);
        }

        private void PlayLevelFinishedSound(LevelResult result)
        {
            switch (result)
            {
                case LevelResult.Victory:
                    _audioManager.PlayOneShot(_audioConfig.LevelCompleted);
                    break;
                case LevelResult.Defeat:
                    _audioManager.PlayOneShot(_audioConfig.LevelFailed);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(result), result, null);
            }
        }

        private void PlayLevelStartedSound()
        {
            _audioManager.PlayOneShot(_audioConfig.LevelStarted);
        }

        private void PlayTheme() => _audioManager.PlayMusic(_audioConfig.Theme);
    }
}
