using System;
using Constants;
using Gameplay;
using Gameplay.Tray;
using UI.Tray;
using UnityEngine;
using Zenject;

namespace Presenters__Controllers
{
    public class TileTrayPresenter : IInitializable, IDisposable
    {
        private readonly TileTray _tileTray;
        private readonly TileTrayView _tileTrayView;
        private readonly TileClickDetector _clickDetector;
        private readonly Camera _mainCamera;
        
        private Vector2 _lastClickedTilePosition;
        
        public TileTrayPresenter(TileTray tileTray,
            TileTrayView tileTrayView,
            TileClickDetector clickDetector,
            Camera mainCamera)
        {
            _tileTray = tileTray;
            _tileTrayView = tileTrayView;
            _clickDetector = clickDetector;
            _mainCamera = mainCamera;
        }

        public void Initialize()
        {
            _clickDetector.TileClicked += SetLastClickedTile;
            _tileTray.Added += Insert;
            _tileTray.MatchCleared += _tileTrayView.Match;
            _tileTray.Cleared += _tileTrayView.Clear;
        }

        public void Dispose()
        {
            _clickDetector.TileClicked -= SetLastClickedTile;
            _tileTray.Added -= Insert;
            _tileTray.MatchCleared -= _tileTrayView.Match;
            _tileTray.Cleared -= _tileTrayView.Clear;
        }
        
        private void SetLastClickedTile(Tile tile) =>
            _lastClickedTilePosition = tile.transform.position;

        private void Insert(TileConfig config, int index)
        {
            var scaleMultiplier = CameraConstants.DefaultOrthographicSize / _mainCamera.orthographicSize;
            var startScale = Vector2.one * scaleMultiplier;
            _tileTrayView.Insert(config, index, _lastClickedTilePosition, startScale);
        }
    }
}
