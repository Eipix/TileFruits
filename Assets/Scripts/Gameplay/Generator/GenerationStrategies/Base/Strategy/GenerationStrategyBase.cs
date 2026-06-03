using System;
using System.Linq;
using Generator;
using Generator.GenerationStrategies;
using UnityEngine;

using static Constants.MahjongConstants;

namespace Gameplay.Generator.GenerationStrategies.Base
{
    public abstract class GenerationStrategyBase : IGenerationStrategy
    {
        protected GenerationStrategyConfigBase Config { get; }
        protected global::Generator.TileMap Map { get; private set; }
        protected Vector2Int Size { get; private set; }
        
        protected bool IsSlotsCountSolvable => Map.Count % TilesPerMatch == 0;
        
        public GenerationStrategyBase(GenerationStrategyConfigBase config, Vector2Int size)
        {
            Config = config;
            Size = size;
        }

        public global::Generator.TileMap GenerateMap()
        {
            Map = new(Size);
            OnGenerateShape();

            if (Map.Count < TilesPerMatch)
                throw new InvalidOperationException($"At least {TilesPerMatch} slots are required");
            
            while (IsSlotsCountSolvable is false)
                RemoveFromTopMost();
            
            return Map;
        }

        private void RemoveFromTopMost()
        {
            var lastSlotPosition = Map.Positions.Last();

            Debug.LogWarning($"Slots count ({Map.Count}) must be divided by {TilesPerMatch}. Removing {lastSlotPosition} slot");
            Map.Remove(lastSlotPosition);
        }

        protected abstract void OnGenerateShape();
    }
}
