using _Commons.Scripts.Effects.Shakers;
using Commons.Extensions;
using DG.Tweening;
using Gameplay;

namespace View.Animations
{
    public class TileShaker
    {
        private ShakerConfig _config;
        private Tween _shaking;

        public TileShaker(ShakerConfig config)
        {
            _config = config;
        }

        public void Shake(Tile tile)
        {
            _shaking.CompleteIfActive();
            _shaking = tile.transform.DOShakePosition(_config);
        }
    }
}
