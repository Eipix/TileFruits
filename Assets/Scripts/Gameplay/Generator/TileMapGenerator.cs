using Generator.Provider;

namespace Generator
{
    public class TileMapGenerator
    {
        private TileMapProvider _tileMapProvider;

        public TileMapGenerator(TileMapProvider tileMapProvider)
        {
            _tileMapProvider = tileMapProvider;
        }
        
        public ITileMap GenerateGrid(GeneratorConfig config)
        {
            TileMap map = new(config.MapSize);
            
            var generationStrategy = config.GenerationStrategy.GetStrategy(map);
            generationStrategy.GenerateShape();
            
            var distributionStrategy = config.DistributionStrategy.GetStrategy(map, config.TileList);
            distributionStrategy.Distribute();
            
            _tileMapProvider.ActiveMap = map;
            return map;
        }
    }
}
