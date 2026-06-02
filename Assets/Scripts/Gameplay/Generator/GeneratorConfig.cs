using System;
using Gameplay;
using Gameplay.Generator.GenerationStrategies.Base;
using Generator.DistributionStrategies.Base.Config;
using NaughtyAttributes;
using UnityEngine;

namespace Generator
{
    [Serializable]
    public class GeneratorConfig
    {
        [field: SerializeField, Expandable, AllowNesting]
        public TileList TileList { get; private set; }
        
        [field: SerializeField, Expandable, AllowNesting]
        public GenerationStrategyConfigBase GenerationStrategy { get; private set; }
        
        [field: SerializeField, Expandable, AllowNesting]
        public DistributionStrategyConfigBase DistributionStrategy { get; private set; }
    }
}
