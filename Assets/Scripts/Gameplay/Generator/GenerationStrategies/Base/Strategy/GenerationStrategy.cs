using Generator;

namespace Gameplay.Generator.GenerationStrategies.Base.Strategy
{
    public abstract class GenerationStrategy<T> : GenerationStrategyBase
        where T : GenerationStrategyConfigBase
    {
        protected new T Config { get; }
        
        protected GenerationStrategy(T config, TileMap map) : base(config, map)
        {
            Config = config;
        }
    }
}
