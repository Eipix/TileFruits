using System;
using Generator;
using Generator.GenerationStrategies;

namespace Gameplay.Generator.GenerationStrategies.Base
{
    public abstract class GenerationStrategyConfig<T> : GenerationStrategyConfigBase
        where T : GenerationStrategyBase
    {
        public override IGenerationStrategy GetStrategy(TileMap tileMap)
        {
            return (T)Activator.CreateInstance(typeof(T), this, tileMap);
        }
    }
}
