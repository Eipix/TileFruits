using System;
using Gameplay;
using Generator.DistributionStrategies.Base.Config;
using Generator.GenerationStrategies.Base;
using NaughtyAttributes;
using UnityEngine;

namespace Generator
{
    [Serializable]
    public class GeneratorConfig
    {
        [field: SerializeField, Expandable, AllowNesting]
        public TileList TileList { get; private set; }

        [field: SerializeReference, SerializeReferenceDropdown, AllowNesting]
        public GenerationStrategy ShapeStrategy { get; private set; }
        
        [field: SerializeField, Expandable, AllowNesting]
        public DistributionStrategyConfigBase DistributionStrategy { get; private set; }
    }
}
