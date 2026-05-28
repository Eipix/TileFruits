using System.Collections.Generic;
using Gameplay;
using NaughtyAttributes;
using UnityEngine;
using Zenject;

namespace Generator
{
    public class MapVisualizer : MonoBehaviour, IInitializable
    {
        private const int LayerPriority = 10;
        
        private readonly List<Tile> _tiles = new();

        [SerializeField] private bool _autoCentering;
        
        [field: SerializeField, OnValueChanged(nameof(RecalculatePosition)), HideIf(nameof(_autoCentering))]
        public Vector2 Center { get; private set; }
        
        [field: SerializeField, OnValueChanged(nameof(RecalculatePosition))]
        public float PaddingBetweenTiles { get; private set; }
        
        [Inject] private Tile.Pool _tilePool;

        private Vector2 _initialPosition;
        
        public void Initialize() => _initialPosition = transform.position;

        private void RecalculatePosition()
        {
            foreach (var tile in _tiles)
                tile.transform.position = (Center + tile.GridPosition) * PaddingBetweenTiles;
        }

        public void CreateTiles(ITileMap tileMap)
        {
            Clear();
            
            foreach (var (position, slot) in tileMap)
                CreateTileView(position, slot);

            if (_autoCentering)
                transform.position = _initialPosition - (Vector2)tileMap.Size / 4;
        }

        private void CreateTileView(Vector3Int position, IReadOnlySlot slot)
        {
            var gridPosition2D = (Vector2Int)position;
            Vector2 spawnPosition = (Vector2)gridPosition2D * PaddingBetweenTiles;

            var layer = (position.z * LayerPriority) - position.y;
            
            var tile = _tilePool.Spawn(slot.Tile, gridPosition2D, spawnPosition, layer, transform);
            _tiles.Add(tile);
        }

        private void Clear()
        {
            foreach (var tile in _tiles)
                _tilePool.Despawn(tile);
            
            _tiles.Clear();
        } 
    }
}
