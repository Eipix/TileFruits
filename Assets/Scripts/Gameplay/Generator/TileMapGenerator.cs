using Gameplay.Tray;
using Generator.DistributionStrategies.Base;
using Generator.Provider;

namespace Generator
{
    public class TileMapGenerator
    {
        private readonly TileMapProvider _tileMapProvider;
        private readonly TileTraySettings _traySettings;

        public TileMapGenerator(TileMapProvider tileMapProvider, TileTraySettings settings)
        {
            _tileMapProvider = tileMapProvider;
            _traySettings = settings;
        }
        
        public ITileMap GenerateGrid(GeneratorConfig config)
        {
            var map = config.ShapeStrategy.GenerateMap();

            DistributionSettings settings = new(map, config.TileList, _traySettings);
            var distributionStrategy = config.DistributionStrategy.GetStrategy(settings);
            distributionStrategy.Distribute();
            
            _tileMapProvider.ActiveMap = map;
            return map;
        }
    }
}
