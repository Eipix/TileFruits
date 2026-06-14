using System;
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

        private Vector2 _lastClickedTilePosition;
        
        public TileTrayPresenter(TileTray tileTray,
            TileTrayView tileTrayView,
            TileClickDetector clickDetector)
        {
            _tileTray = tileTray;
            _tileTrayView = tileTrayView;
            _clickDetector = clickDetector;
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
            _tileTrayView.Insert(config, index, _lastClickedTilePosition);
        }
    }
}
