using UnityEngine;
using Zenject;

namespace Generator
{
    public class TileMapGenerator
    {
        private readonly TileFactory _factory;
        private readonly TileMapGeneratorSettings _settings;
        
        public Vector2 Center => _settings.Center;
        public float PaddingBetweenTiles => _settings.PaddingBetweenTiles;

        public TileMapGenerator(TileFactory tileFactory, TileMapGeneratorSettings settings)
        {
            _factory = tileFactory;
            _settings = settings;
        }

        public void GenerateGrid(GeneratorConfig config)
        {
            TileMap map = new(_settings.MapSize);
            
        }
    }
}
