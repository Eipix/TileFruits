using System;
using DG.Tweening;
using UnityEngine;
using View.Animations;
using Zenject;

namespace Gameplay
{
    [SelectionBase]
    public class Tile : MonoBehaviour
    {
        [SerializeField] private SpriteRenderer _bone;
        [SerializeField] private SpriteRenderer _symbol;

        private Tween _showAnimation;
        private Color _color;
        private Vector2 _initialScale;
        
        public Color Color
        {
            get => _color;
            set
            {
                _color = value;
                _bone.color = value;
                _symbol.color = value;
            }
        }
        
        public TileConfig Config { get; private set; }
        public Vector3Int GridPosition { get; private set; }

        public int SortingOrder => _bone.sortingOrder;
        public Vector2 Size => _bone.sprite.bounds.size;

        private void Awake() => _initialScale = transform.localScale;

        private void SetLayer(int layer)
        {
            _bone.sortingOrder = layer;
            _symbol.sortingOrder = layer + 1;
        }

        public Tween StartShowing(ShowTileAnimationConfig config, float delay)
        {
            transform.localScale = Vector2.zero;
            
            _showAnimation = transform.DOScale(_initialScale, config.Duration)
                .SetEase(config.Ease, config.OverShoot)
                .SetDelay(delay)
                .SetLink(gameObject, LinkBehaviour.CompleteOnDisable);
            
            return _showAnimation;
        }

        public class Pool : MonoMemoryPool<TileConfig, Vector3Int, Vector2, int, Transform, Tile>
        {
            protected override void Reinitialize(TileConfig config, Vector3Int gridPosition,
                Vector2 position, int layer, Transform transform, Tile tile)
            {
                base.Reinitialize(config, gridPosition, position, layer, transform, tile);
                tile.SetLayer(layer);
                tile.Config = config;
                tile._symbol.sprite = config.Symbol;
                tile.GridPosition = gridPosition;
                tile.transform.position = position;
                tile.transform.SetParent(transform);
                tile.Color = Color.white;
                
#if UNITY_EDITOR
                tile.name = $"Tile {gridPosition}";
#endif
            }
        }
    }
}
