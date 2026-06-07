using System;
using _Commons.Scripts.UI;
using Gameplay.Levels;
using Gameplay.Tray;
using Generator;
using Generator.Provider;
using UI;
using Zenject;

namespace Gameplay
{
    public class GameplayController : IDisposable
    {
        private readonly TileClickDetector _tileClickDetector;
        private readonly UIManager _uiManager;
        private readonly LevelManager _levelManager;
        private readonly ITileMapProvider _tileMapProvider;
        private readonly TileTray _tray;

        private ITileMap Map => _tileMapProvider.ActiveMap;

        public GameplayController(
            TileClickDetector tileClickDetector,
            UIManager uiManager,
            LevelManager levelManager,
            ITileMapProvider tileMapProvider,
            TileTray tray)
        {
            _tileClickDetector = tileClickDetector;
            _uiManager = uiManager;
            _levelManager = levelManager;
            _tileMapProvider = tileMapProvider;
            _tray = tray;
        }

        public void Initialize()
        {
            _tileClickDetector.TileClicked += OnTileClicked;
            _levelManager.LevelFinished += OnLevelFinished;
        }

        public void Dispose()
        {
            _tileClickDetector.TileClicked -= OnTileClicked;
            _levelManager.LevelFinished -= OnLevelFinished;
        }
        
        private void OnLevelFinished(LevelResult result)
        {
            switch (result)
            {
                case LevelResult.Victory:
                    _uiManager.OpenWindow<VictoryWindow>();
                    break;
                case LevelResult.Defeat:
                    _uiManager.OpenWindow<DefeatWindow>();
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(result), result, null);
            }
        }

        private void OnTileClicked(Tile tile)
        {
            if (Map.TryTakeTile(tile.GridPosition))
            {
                _tray.Add(tile.Config);
            }
        }
    }
}
