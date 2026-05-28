using Gameplay.Generator.GenerationStrategies.Base;

namespace Generator.GenerationStrategies.Base
{
    public abstract class GenerationStrategyConfig<T> : GenerationStrategyConfigBase
        where T : IGenerationStrategy
    {
    }
}
