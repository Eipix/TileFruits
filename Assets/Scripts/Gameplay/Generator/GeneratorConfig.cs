using System;
using System.Collections.Generic;
using Gameplay;
using Gameplay.Generator.GenerationStrategies.Base;
using Generator.DistributionStrategies.Base.Config;
using Generator.GenerationStrategies.Base;
using Generator.GenerationStrategies.Implementations;
using NaughtyAttributes;
using UnityEngine;

using LegacyCustomStrategy = Generator.GenerationStrategies.Implementations.Custom.CustomStrategyConfig;

namespace Generator
{
    [Serializable]
    public class GeneratorConfig
    {
        [field: SerializeField, Expandable, AllowNesting]
        public TileList TileList { get; private set; }

        [field: SerializeReference, SerializeReferenceDropdown, AllowNesting]
        public GenerationStrategy ShapeStrategy { get; private set; }
        
        [field: SerializeField, HideInInspector]
        public GenerationStrategyConfigBase GenerationStrategy { get; private set; }
        
        [field: SerializeField, Expandable, AllowNesting]
        public DistributionStrategyConfigBase DistributionStrategy { get; private set; }

        public void MigrateCustomStrategyIfNull()
        {
            if (ShapeStrategy == null
                && GenerationStrategy is LegacyCustomStrategy customStrategy)
            {
                ShapeStrategy = new CustomStrategy(customStrategy.Size,
                    (List<Vector3Int>)customStrategy.Positions);
            }
        }
    }
}
