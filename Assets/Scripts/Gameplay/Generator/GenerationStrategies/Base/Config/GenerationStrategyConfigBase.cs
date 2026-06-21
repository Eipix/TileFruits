using System;
using Generator.GenerationStrategies;
using UnityEngine;

namespace Gameplay.Generator.GenerationStrategies.Base
{
    [Obsolete("This class is obsolete and will be removed in the next major update. Use non-generic GenerationStrategy instead.")]
    public abstract class GenerationStrategyConfigBase : ScriptableObject
    {
        [field: SerializeField]
        public Vector2Int Size { get; protected set; } = Vector2Int.one * 5;
        
        public abstract IGenerationStrategy GetStrategy();
    }
}
