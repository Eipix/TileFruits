using System;

namespace Generator
{
    public class TileMapGenerator
    {
        public event Action<ITileMap> Generated;
        
        public ITileMap GenerateGrid(GeneratorConfig config)
        {
            TileMap map = new(config.MapSize);
            
            var generationStrategy = config.GenerationStrategy.GetStrategy(map);
            generationStrategy.GenerateShape();
            
            var distributionStrategy = config.DistributionStrategy.GetStrategy(map, config.TileList);
            distributionStrategy.Distribute();
            
            Generated?.Invoke(map);
            return map;
        }
    }
}
