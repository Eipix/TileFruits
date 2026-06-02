using UnityEngine;
using Zenject;

namespace Gameplay
{
    [SelectionBase]
    public class Tile : MonoBehaviour
    {
        [SerializeField] private SpriteRenderer _bone;
        [SerializeField] private SpriteRenderer _symbol;
        
        public TileConfig Config { get; private set; }
        public Vector3Int GridPosition { get; private set; }

        public int SortingOrder => _bone.sortingOrder;

        private void SetLayer(int layer)
        {
            _bone.sortingOrder = layer;
            _symbol.sortingOrder = layer + 1;
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

                tile.name = $"Tile {gridPosition}";
            }
        }
    }
}
