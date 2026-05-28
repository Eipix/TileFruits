using Gameplay.Generator.GenerationStrategies.Base;
using NaughtyAttributes;
using UnityEngine;

namespace Generator.GenerationStrategies.PyramidStrategy
{
    [CreateAssetMenu(menuName = "Generator/GenerationStrategies/Pyramid")]
    public class PyramidStrategyConfig : GenerationStrategyConfig<PyramidStrategy>
    {
        [field: SerializeField] public bool UnlimitedLayers { get; private set; }
        
        [field: SerializeField, Min(1), HideIf(nameof(UnlimitedLayers))]
        public int MaxLayers { get; private set; } = 5;
    }
}
