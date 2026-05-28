using Generator.GenerationStrategies.Base;
using UnityEngine;

namespace Generator.GenerationStrategies.PyramidStrategy
{
    [CreateAssetMenu(menuName = "Tiles/Generator/Strategies/PyramidStrategy")]
    public class PyramidStrategyConfig : GenerationStrategyConfig<PyramidStrategy>
    {
        public override IGenerationStrategy GetStrategy()
        {
            return new PyramidStrategy();
        }
    }
}
