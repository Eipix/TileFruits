using System;
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
        
        private readonly Dictionary<Vector3Int, Tile> _tiles = new();

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
            foreach (var tile in _tiles.Values)
                tile.transform.position = ((Vector3)Center + tile.GridPosition) * PaddingBetweenTiles;
        }

        public void CreateTiles(ITileMap tileMap)
        {
            Clear((() => tileMap.Taken -= OnTileTaken));
            
            tileMap.Taken += OnTileTaken;
            
            foreach (var (position, slot) in tileMap)
                CreateTileView(position, slot);

            if (_autoCentering)
            {
                var targetPosition = _initialPosition - (Vector2)tileMap.Size / 4;
                transform.position = targetPosition;
            }
            
            RecalculatePosition();
        }

        private void OnTileTaken(Vector3Int position)
        {
            var tile = _tiles[position];
            _tilePool.Despawn(tile);
            _tiles.Remove(position);
        }

        private void CreateTileView(Vector3Int position, IReadOnlySlot slot)
        {
            Vector2 spawnPosition = (Vector3)position * PaddingBetweenTiles;
            var layer = (position.z * LayerPriority) - position.y;
            
            var tile = _tilePool.Spawn(slot.Tile, position, spawnPosition, layer, transform);
            _tiles.Add(position, tile);
        }

        private void Clear(Action onCleared)
        {
            foreach (var tile in _tiles.Values)
                _tilePool.Despawn(tile);
            
            _tiles.Clear();
            onCleared?.Invoke();
        } 
    }
}
