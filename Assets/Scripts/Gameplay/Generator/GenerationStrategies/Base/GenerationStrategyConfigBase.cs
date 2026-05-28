using Generator.GenerationStrategies;
using UnityEngine;

namespace Gameplay.Generator.GenerationStrategies.Base
{
    public abstract class GenerationStrategyConfigBase : ScriptableObject
    {
        public abstract IGenerationStrategy GetStrategy();
    }
}
