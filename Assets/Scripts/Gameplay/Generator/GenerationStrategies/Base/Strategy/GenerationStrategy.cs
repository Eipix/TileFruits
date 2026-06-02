using UnityEngine;

namespace Gameplay.Generator.GenerationStrategies.Base.Strategy
{
    public abstract class GenerationStrategy<T> : GenerationStrategyBase
        where T : GenerationStrategyConfigBase
    {
        protected new T Config { get; }
        
        protected GenerationStrategy(T config, Vector2Int size) : base(config, size)
        {
            Config = config;
        }
    }
}
