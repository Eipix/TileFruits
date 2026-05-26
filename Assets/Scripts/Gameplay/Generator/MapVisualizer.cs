using System;
using System.Collections.Generic;
using Gameplay;
using UnityEngine;
using Zenject;

namespace Generator
{
    public class MapVisualizer : MonoBehaviour
    {
        [field: SerializeField] public Vector2 Center { get; private set; }
        [field: SerializeField] public float PaddingBetweenTiles { get; private set; }
        
        private readonly List<Tile> _tiles = new();
        
        private TileMapGenerator _generator;
        private TileFactory _factory;
        
        [Inject]
        private void Construct(TileMapGenerator generator,
            TileFactory factory)
        {
            _generator = generator;
            _factory = factory;
        }

        private void OnEnable() => _generator.Generated += OnGenerated;
        private void OnDisable() => _generator.Generated -= OnGenerated;

        private void OnGenerated(ITileMap tileMap)
        {
            foreach (var (position, slot) in tileMap)
            {
                if (slot.IsEmpty)
                    throw new InvalidOperationException($"Slot ({position}) can't be empty");

                CreateTileView(position, slot);
            }
        }

        private void CreateTileView(Vector3Int position, IReadOnlySlot slot)
        {
            Vector2 spawnPosition = (Center + (Vector2Int)position) * PaddingBetweenTiles;
            var tile = _factory.Create(slot.Tile, spawnPosition, transform, position.z);
            _tiles.Add(tile);
        }
    }
}
