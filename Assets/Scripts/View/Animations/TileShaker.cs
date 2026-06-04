using System;
using _Commons.Scripts.Effects.Shakers;
using Commons.Extensions;
using DG.Tweening;
using Gameplay;
using Zenject;

namespace View.Animations
{
    public class TileShaker : IInitializable, IDisposable
    {
        private readonly TileClickDetector _tileClickDetector;
        
        private ShakerConfig _config;
        private Tween _shaking;

        public TileShaker(TileClickDetector detector, ShakerConfig config)
        {
            _tileClickDetector = detector;
            _config = config;
        }

        public void Initialize() => _tileClickDetector.TileClicked += Shake;

        public void Dispose() => _tileClickDetector.TileClicked -= Shake;

        private void Shake(Tile tile)
        {
            _shaking.CompleteIfActive();
            _shaking = tile.transform.DOShakePosition(_config);
        }
    }
}
