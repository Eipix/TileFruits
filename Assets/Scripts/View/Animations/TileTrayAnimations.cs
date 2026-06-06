using System;
using DG.Tweening;
using Gameplay;
using Gameplay.Tray;
using UI.Tray;
using UnityEngine;
using IInitializable = Zenject.IInitializable;

namespace View.Animations
{
    public class TileTrayAnimations : IInitializable, IDisposable
    {
        private readonly CollectAnimationConfig _config;
        private readonly TileClickDetector _tileClickDetector;
        private readonly TileTrayView _trayView;
        
        private Vector2 _lastClickedTilePosition;

        public TileTrayAnimations(
            CollectAnimationConfig config,
            TileClickDetector clickDetector,
            TileTrayView trayView)
        {
            _config = config;
            _tileClickDetector = clickDetector;
            _trayView = trayView;
        }
        
        public void Initialize()
        {
            _tileClickDetector.TileClicked += SetLastClickedTile;
            _trayView.Added += Start;
        }

        public void Dispose()
        {
            _tileClickDetector.TileClicked -= SetLastClickedTile;
            _trayView.Added -= Start;
        }

        private void SetLastClickedTile(Tile tile)
        {
            _lastClickedTilePosition = tile.transform.position;
            Debug.Log($"last clicked position: {_lastClickedTilePosition} grid pos {tile.GridPosition}");
        }

        private void Start(TileTrayItem item)
        {
            var iconsParent = item.IconsParent;
            iconsParent.position = _lastClickedTilePosition;

            Debug.Log($"Set position {iconsParent.position}");
            DOTween.Sequence()
                .Append(iconsParent.DOAnchorPos(Vector2.zero, _config.MoveDuration)
                    .SetEase(_config.MoveEase))
                .Append(iconsParent.DOPunchScale(_config.Punch, _config.PunchDuration,
                    _config.Vibrato, _config.Elasticity));
        }
    }
}
