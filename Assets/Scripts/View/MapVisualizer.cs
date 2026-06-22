using System;
using System.Collections.Generic;
using Commons.Utils;
using UnityEngine;
using View.Animations;
using Zenject;
using Tile = Gameplay.Tile;

namespace Generator
{
    public class MapVisualizer : MonoBehaviour
    {
        public const int LayerPriority = 10;
        
        private readonly Dictionary<Vector3Int, Tile> _tiles = new();

        [SerializeField] private Color _blockedTilesColor = Color.gray;
        [SerializeField] private float _layerOffset = 0.16f;
        [SerializeField] private float _paddingBetweenTiles = 0.6f;
        
        public event Action<Tile> TileTaken;
        
        private Tile.Pool _tilePool;
        private ShowTileAnimationConfig _showTileConfig;

        public Vector2 Size { get; private set; }

        [Inject]
        private void Construct(Tile.Pool tilePool, ShowTileAnimationConfig showTileConfig)
        {
            _tilePool = tilePool;
            _showTileConfig = showTileConfig;
        }
        
        public void SpawnTiles(ITileMap tileMap)
        {
            Clear((() => tileMap.TileTaken -= OnTileTaken));
            
            tileMap.TileTaken += OnTileTaken;

            int i = 0;
            foreach (var (position, slot) in tileMap)
            {
                var tile = Spawn(position, slot);
                tile.StartShowing(_showTileConfig, _showTileConfig.StepDelay * position.z);
                i++;
            }
            
            Center();
            RepaintBlockedTiles(tileMap);
            
            void OnTileTaken(Vector3Int gridPosition) =>
                DespawnAndRecolorLowerTiles(tileMap, gridPosition);
        }
        
        private void DespawnAndRecolorLowerTiles(ITileMap tileMap, Vector3Int gridPosition)
        {
            Despawn(gridPosition);
            
            var lowerPosition = gridPosition;
            lowerPosition.z--;

            if (lowerPosition.z < 0)
                return;

            if (_tiles.TryGetValue(lowerPosition, out var tile))
                tile.Color = Color.white;
            
            foreach (var direction in TileMapUtils.DirectionsAround)
            {
                var position = lowerPosition + direction;

                if (tileMap.CanTakeTile(position))
                    _tiles[position].Color = Color.white;
            }
        }

        private void RepaintBlockedTiles(ITileMap tileMap)
        {
            foreach (var (position, tile) in _tiles)
            {
                if(tileMap.IsBlockedByAbove(position))
                    tile.Color = _blockedTilesColor;
            }
        }

        private void Despawn(Vector3Int gridPosition)
        {
            var tile = _tiles[gridPosition];
            _tilePool.Despawn(tile);
            _tiles.Remove(gridPosition);

            TileTaken?.Invoke(tile);
        }

        private Tile Spawn(Vector3Int gridPosition, IReadOnlySlot slot)
        {
            Vector2 spawnPosition = (Vector3)gridPosition * _paddingBetweenTiles;
            
            if(gridPosition.z > 0)
                spawnPosition.y += _layerOffset * gridPosition.z;
            
            var layer = (gridPosition.z * LayerPriority) - gridPosition.y;
            
            var tile = _tilePool.Spawn(slot.Tile, gridPosition, spawnPosition, layer, transform);
            _tiles.Add(gridPosition, tile);
            return tile;
        }

        private void Clear(Action onCleared)
        {
            foreach (var tile in _tiles.Values)
                _tilePool.Despawn(tile);
            
            _tiles.Clear();
            onCleared?.Invoke();
        }

        private void Center()
        {
            if (_tiles.Count == 0)
            {
                Size = Vector2.one;
                return;
            }

            float minX = float.MaxValue;
            float maxX = float.MinValue;
            float minY = float.MaxValue;
            float maxY = float.MinValue;

            foreach (var tile in _tiles.Values)
            {
                Vector2 position = tile.transform.localPosition;
                Vector2 size = tile.Size;

                float tileMinX = position.x - size.x / 2f;
                float tileMaxX = position.x + size.x / 2f;
                float tileMinY = position.y - size.y / 2f;
                float tileMaxY = position.y + size.y / 2f;

                if (tileMinX < minX)
                    minX = tileMinX;
                
                if (tileMaxX > maxX)
                    maxX = tileMaxX;
                
                if (tileMinY < minY)
                    minY = tileMinY;
                
                if (tileMaxY > maxY)
                    maxY = tileMaxY;
            }

            Size = new Vector2(maxX - minX, maxY - minY);
            Vector2 localCenter = new Vector2((minX + maxX) / 2f, (minY + maxY) / 2f);
            transform.position = -localCenter;
        }
    }
}
