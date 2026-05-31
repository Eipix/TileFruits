using System;
using Gameplay.Tray;
using Generator;
using Generator.Provider;
using UnityEngine;
using Zenject;

namespace Gameplay
{
    public class GameplayController : IInitializable, IDisposable
    {
        private readonly TileClickDetector _tileClickDetector;
        private readonly TileTray _tray;
        
        private ITileMapProvider _tileMapProvider;
        
        private ITileMap Map => _tileMapProvider.ActiveMap;

        public GameplayController(
            ITileMapProvider tileMapProvider,
            TileClickDetector tileClickDetector,
            TileTray tray)
        {
            _tileMapProvider = tileMapProvider;
            _tileClickDetector = tileClickDetector;
            _tray = tray;
        }
        
        public void Initialize()
        {
            _tileClickDetector.TileClicked += OnTileClicked;
            _tray.Filled += OnGameOver;
        }

        private void OnTileClicked(Tile tile)
        {
            if (Map.TryTakeTile(tile.GridPosition))
            {
                _tray.Add(tile.Config);
            }
        }

        private void OnGameOver()
        {
            Debug.LogWarning("GameOver");
        }

        public void Dispose()
        {
            _tileClickDetector.TileClicked -= OnTileClicked;
            _tray.Filled -= OnGameOver;
        }
    }
}
