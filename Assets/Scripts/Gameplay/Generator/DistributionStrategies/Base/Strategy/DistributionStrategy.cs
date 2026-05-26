using Gameplay;
using Generator.DistributionStrategies.Base.Config;

namespace Generator.DistributionStrategies
{
    public abstract class DistributionStrategy<T> : DistributionStrategyBase
        where T : DistributionStrategyConfigBase
    {
        protected new T Config { get; }

        public DistributionStrategy(T config, TileMap map, TileList tileList) : base(config, map, tileList)
        {
            Config = config;
        }
    }
}
