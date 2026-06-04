using Gameplay.Generator.GenerationStrategies.Base;
using UnityEngine;

namespace Generator.GenerationStrategies.Implementations.Fullfill
{    
    [CreateAssetMenu(menuName = "Generator/GenerationStrategies/Fullfill")]
    public class FullfillStrategyConfig : GenerationStrategyConfig<FullfillStrategy>
    {
        [field: SerializeField, Min(1)] public int Layers { get; private set; }
    }
}
