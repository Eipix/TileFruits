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
        protected TileMap Map { get; }
        protected Vector2Int Size => Map.Size;
        
        protected bool IsSlotsCountEven => Map.Count % TilesPerMatch == 0;
        
        public GenerationStrategyBase(GenerationStrategyConfigBase config, TileMap map)
        {
            Config = config;
            Map = map;
        }

        public void GenerateShape()
        {
            OnGenerateShape();

            if (Map.Count < TilesPerMatch)
                throw new InvalidOperationException($"At least {TilesPerMatch} slots are required");
            
            while (IsSlotsCountEven is false)
                RemoveFromTopMost();
        }

        private void RemoveFromTopMost()
        {
            var lastSlotPosition = Map.Positions.Last();

            Debug.LogWarning($"Slots count ({Map.Count}) must be even. Removing {lastSlotPosition} slot");
            Map.Remove(lastSlotPosition);
        }

        protected abstract void OnGenerateShape();
    }
}
