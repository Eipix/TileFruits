using System;
using System.Linq;
using NaughtyAttributes;
using UnityEngine;

using static Constants.MahjongConstants;

namespace Generator.GenerationStrategies.Base
{
    [Serializable]
    public abstract class GenerationStrategy : IGenerationStrategy
    {
        [field: SerializeField]
        public Vector2Int Size { get; protected set; } = Vector2Int.one * 10;
        
        protected TileMap Map { get; private set; }
        protected bool IsSlotsCountSolvable => Map.Count % TilesPerMatch == 0;

        public TileMap GenerateMap()
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
