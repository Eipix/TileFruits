using System;
using Gameplay;
using Gameplay.Generator.GenerationStrategies.Base;
using NaughtyAttributes;
using UnityEngine;

namespace Generator
{
    [Serializable]
    public class GeneratorConfig
    {
        [field: SerializeField, ShowAssetPreview]
        public TileList TileList { get; private set; }
        
        [field: SerializeField, ShowAssetPreview]
        public GenerationStrategyConfigBase GenerationStrategy { get; private set; }
    }
}
