using System;
using System.Collections.Generic;
using Commons.Extensions;
using Commons.Utils;
using Constants;
using Gameplay.Generator.GenerationStrategies.Base;
using NaughtyAttributes;
using UnityEngine;

namespace Generator.GenerationStrategies.Implementations.Custom
{
    [CreateAssetMenu(menuName = "Generator/GenerationStrategies/Custom")]
    public class CustomStrategyConfig : GenerationStrategyConfig<CustomStrategy>
    {
        [ShowNativeProperty] private Vector2Int ExpectedSize => Size;
        
        [SerializeField] private List<Vector3Int> _positions;
        
        public IReadOnlyList<Vector3Int> Positions => _positions;

        protected override bool HideSize => true;

        private void OnValidate() => Size = FindSize();
        
        private Vector2Int FindSize()
        {
            Vector2Int size = Vector2Int.one;
            
            foreach (var position in _positions)
            {
                if (position.x > size.x)
                    size.x = position.x;
                
                if (position.y > size.y)
                    size.y = position.y;
            }
            
            return size;
        }

        [Button]
        private bool IsValid()
        {
            if (IsValid(out var error))
            {
                Debug.Log($"<color=green>{name} strategy is valid!</color>");
                return true;
            }
            
            Debug.LogError(error);
            return false;
        }
        
        public bool IsValid(out string errorMessage)
        {
            errorMessage = string.Empty;
            
            if (_positions.Count is 0)
            {
                errorMessage = "No positions have been generated";
                return false;
            }

            bool isSolvable = _positions.Count % MahjongConstants.TilesPerMatch is 0;

            if (isSolvable is false)
            {
                errorMessage = $"Invalid positions count (must be divided by {MahjongConstants.TilesPerMatch})";
                return false;
            }
                
            HashSet<Vector3Int> positionsLookup = new(_positions.Count);
            
            foreach (var position in _positions)
            {
                if (positionsLookup.Add(position) is false) 
                {
                    errorMessage = $"Duplicate positions are not allowed: {position}";
                    return false;
                }
                
                if(position.x < 0 || position.y < 0 || position.z < 0)
                {
                    errorMessage = $"Position cannot be negative {position}";
                    return false;
                }
            }
            
            foreach (var position in _positions)
            {
                if (HasPositionAround(position, positionsLookup))
                {
                    errorMessage = $"position offset must be 2 - {position}";
                    return false;
                }

                var lowerLayerPosition = position;
                lowerLayerPosition.z--;

                if (lowerLayerPosition.z < 0)
                    continue;

                if (positionsLookup.Contains(lowerLayerPosition) is false
                    && HasPositionAround(lowerLayerPosition, positionsLookup) is false)
                {
                    errorMessage = $"position {position} must have support from below";
                    return false;
                }
            }

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
    }
}
