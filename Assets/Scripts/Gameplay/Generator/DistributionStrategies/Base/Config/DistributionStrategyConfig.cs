using System;
using Gameplay;

namespace Generator.DistributionStrategies.Base.Config
{
    public class DistributionStrategyConfig<T> : DistributionStrategyConfigBase 
        where T : DistributionStrategyBase
    {
        public override IDistributionStrategy GetStrategy(TileMap tileMap, TileList tileList)
        {
            return (T)Activator.CreateInstance(typeof(T), this, tileMap, tileList);
        }
    }
}
