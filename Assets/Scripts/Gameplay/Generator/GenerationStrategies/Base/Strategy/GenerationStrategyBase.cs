using System;
using Generator;
using Generator.GenerationStrategies;
using UnityEngine;

namespace Gameplay.Generator.GenerationStrategies.Base
{
    public abstract class GenerationStrategyBase : IGenerationStrategy
    {
        protected GenerationStrategyConfigBase Config;
        protected TileMap Map { get; private set; }
        protected Vector2Int Size => Map.Size;
        
        protected bool IsSlotsCountEven => Map.Count % 2 == 0;
        
        public GenerationStrategyBase(GenerationStrategyConfigBase config, TileMap map)
        {
            Config = config;
            Map = map;
        }

        public void GenerateShape()
        {
            OnGenerateShape();

            if (Map.Count < 2)
                throw new InvalidOperationException("At least 2 slots are required");
            
            if (IsSlotsCountEven is false)
                RemoveFromTopMost();
        }

        private void RemoveFromTopMost()
        {
            int highestLayer = Map.HighestLayer;
            
            foreach (var position in Map.Positions)
            {
                if(position.z == highestLayer)
                {
                    Map.Remove(position);
                    Debug.LogWarning($"Slots count ({Map.Count}) must be even. Removing {position} slot");
                    break;
                }
            }
        }

        protected abstract void OnGenerateShape();
    }
}
