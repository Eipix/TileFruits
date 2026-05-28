using NaughtyAttributes;
using UnityEngine;
using Zenject;

namespace Gameplay
{
    [SelectionBase]
    public class Tile : MonoBehaviour
    {
        [SerializeField] private SpriteRenderer _bone;
        [SerializeField] private SpriteRenderer _symbol;

        private TileConfig _config;
        
        public Vector2Int GridPosition { get; private set; }

        private void SetLayer(int layer)
        {
            _bone.sortingOrder = layer;
            _symbol.sortingOrder = layer + 1;
        }
        
        [Button(enabledMode: EButtonEnableMode.Playmode)]
        private void LayerUp()
        {
            _bone.sortingOrder++;
            _symbol.sortingOrder++;
        }
        
        [Button(enabledMode: EButtonEnableMode.Playmode)]
        private void LayerDown()
        {
            _bone.sortingOrder--;
            _symbol.sortingOrder--;
        }

        public class Pool : MemoryPool<TileConfig, Vector2Int, Vector2, int, Transform, Tile>
        {
            protected override void Reinitialize(TileConfig config, Vector2Int gridPosition,
                Vector2 position, int layer, Transform transform, Tile tile)
            {
                base.Reinitialize(config, gridPosition, position, layer, transform, tile);
                tile.SetLayer(layer);
                tile._config = config;
                tile._symbol.sprite = config.Symbol;
                tile.GridPosition = gridPosition;
                tile.transform.position = position;
                tile.transform.SetParent(transform);
            }
        }
    }
}
