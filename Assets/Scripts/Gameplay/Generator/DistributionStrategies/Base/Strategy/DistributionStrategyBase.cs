using Gameplay;
using Generator.DistributionStrategies.Base.Config;

namespace Generator.DistributionStrategies
{
    public abstract class DistributionStrategyBase : IDistributionStrategy
    {
        protected DistributionStrategyConfigBase Config;
        protected TileMap Map { get; private set; }
        protected TileList TileList { get; private set; }
        
        public DistributionStrategyBase(DistributionStrategyConfigBase config,
            TileMap map,
            TileList tileList)
        {
            Config = config;
            Map = map;
            TileList = tileList;
        }

        public abstract void Distribute();
    }
}
