using Generator.GenerationStrategies;
using NaughtyAttributes;
using UnityEngine;

namespace Gameplay.Generator.GenerationStrategies.Base
{
    public abstract class GenerationStrategyConfigBase : ScriptableObject
    {
        [field: SerializeField, HideIf(nameof(HideSize))]
        public Vector2Int Size { get; protected set; } = Vector2Int.one * 5;
        
        protected virtual bool HideSize => false;
        
        public abstract IGenerationStrategy GetStrategy();
    }
}
