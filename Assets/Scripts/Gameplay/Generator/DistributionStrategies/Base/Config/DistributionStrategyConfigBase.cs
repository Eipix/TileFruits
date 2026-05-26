using Gameplay;
using UnityEngine;

namespace Generator.DistributionStrategies.Base.Config
{
    public abstract class DistributionStrategyConfigBase : ScriptableObject
    {
        public abstract IDistributionStrategy GetStrategy(TileMap tileMap, TileList tileList);
    }
}
