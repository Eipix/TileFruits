using System;
using Gameplay;
using Gameplay.Tray;
using UI.Tray;
using UnityEngine;
using IInitializable = Zenject.IInitializable;

namespace View.Animations
{
    public class TileTrayAnimationsPresenter : IInitializable, IDisposable
    {
        private readonly TileClickDetector _tileClickDetector;
        private readonly TileTrayView _trayView;
        
        private Vector2 _lastClickedTilePosition;

        public TileTrayAnimationsPresenter(
            TileClickDetector clickDetector,
            TileTrayView trayView)
        {
            _tileClickDetector = clickDetector;
            _trayView = trayView;
        }
        
        public void Initialize()
        {
            _tileClickDetector.TileClicked += SetLastClickedTile;
            _trayView.Added += StartCollecting;
        }

        public void Dispose()
        {
            _tileClickDetector.TileClicked -= SetLastClickedTile;
            _trayView.Added -= StartCollecting;
        }

        private void SetLastClickedTile(Tile tile) =>
            _lastClickedTilePosition = tile.transform.position;

        private void StartCollecting(TileTrayItem item)
        {
            item.SetWorldPosition(_lastClickedTilePosition);
            item.ReturnToTray();
        }
    }
}
