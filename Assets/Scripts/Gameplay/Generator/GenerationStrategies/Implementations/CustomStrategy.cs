using System;
using System.Collections.Generic;
using Commons.Utils;
using Constants;
using Generator.GenerationStrategies.Base;
using UnityEngine;

namespace Generator.GenerationStrategies.Implementations
{
    [Serializable]
    public class CustomStrategy : GenerationStrategy
    {
        [SerializeField, HideInInspector]
        private List<Vector3Int> _positions;

        public CustomStrategy(Vector2Int size, List<Vector3Int> positions)
        {
            Size = size;
            _positions = positions;
        }
        
        public bool IsValidAll()
        {
            if (IsValidAll(out var message))
            {
                Debug.Log(message);
                return true;
            }
            
            Debug.LogError(message);
            return false;
        }
        
        public bool IsValidAll(out string message)
        {
            message = string.Empty;

            if (IsSolvable(out message) is false)
                return false;
                
            HashSet<Vector3Int> positionsLookup = new(_positions.Count);
            
            foreach (var position in _positions)
            {
                if (positionsLookup.Add(position) is false) 
                {
                    message = $"Duplicate positions are not allowed: {position}";
                    return false;
                }
                
                if(position.x < 0 || position.y < 0 || position.z < 0)
                {
                    message = $"Position cannot be negative {position}";
                    return false;
                }
            }
            
            foreach (var position in _positions)
            {
                if (HasPositionAround(position, positionsLookup))
                {
                    message = $"position offset must be 2 - {position}";
                    return false;
                }

                var lowerLayerPosition = position;
                lowerLayerPosition.z--;

                if (lowerLayerPosition.z < 0)
                    continue;

                if (positionsLookup.Contains(lowerLayerPosition) is false
                    && HasPositionAround(lowerLayerPosition, positionsLookup) is false)
                {
                    message = $"position {position} must have support from below";
                    return false;
                }
            }

            message = $"<color=green>{GetType().Name} strategy is valid!</color>";
            return true;
        }
        
        public bool IsSolvable(out string message)
        {
            message = string.Empty;
            
            if (_positions.Count is 0)
            {
                message = "No positions have been generated";
                return false;
            }

            bool isSolvable = _positions.Count % MahjongConstants.TilesPerMatch is 0;

            if (isSolvable is false)
            {
                message = $"Invalid positions count (must be divided by {MahjongConstants.TilesPerMatch})";
                return false;
            }
            
            message = "Strategy is solvable!";
            return true;
        }

        private bool HasPositionAround(Vector3Int center, HashSet<Vector3Int> lookup) =>
            HasPositionInDirections(center, TileMapUtils.DirectionsAround, lookup);
        
        private bool HasPositionInDirections(Vector3Int center, ReadOnlySpan<Vector3Int> directions, HashSet<Vector3Int> lookup)
        {
            foreach (var direction in directions)
            {
                var targetPosition = center + direction;
                
                if (lookup.Contains(targetPosition))
                    return true;
            }

            return false;
        }

        protected override void OnGenerateShape()
        {
            Map.AddRange(_positions);
        }
    }
}
