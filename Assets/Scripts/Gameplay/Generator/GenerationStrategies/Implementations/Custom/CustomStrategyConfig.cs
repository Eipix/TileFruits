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
        public bool IsValid()
        {
            if (IsValid(out var error))
            {
                Debug.Log($"<color=green>{name} strategy is valid!</color>");
                return true;
            }
            
            Debug.LogError(error);
            return false;
        }
        
        private bool IsValid(out string errorMessage)
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
                
            if (_positions.HasDuplicate(p => p))
            {
                errorMessage = "Duplicate positions are not allowed";
                return false;
            }
            
            foreach (var position in _positions)
            {
                if(IsValidPosition(position, out errorMessage) is false)
                    return false;
            }

            return true;
        }

        private bool IsValidPosition(Vector3Int position, out string errorMessage)
        {
            if(position.x < 0 || position.y < 0)
            {
                errorMessage = $"Position cannot be negative {position}";
                return false;
            }
            
            if(HasPositionInDirections(position, TileMapUtils.DirectionsAround))
            {
                errorMessage = $"position offset must be 2 - {position}";
                return false;
            }
            
            errorMessage = null;
            return true;
        }
        
        private bool HasPositionInDirections(Vector3Int center, ReadOnlySpan<Vector3Int> directions)
        {
            foreach (var direction in directions)
            {
                if(HasPositionInDirection(center, direction))
                    return true;
            }

            return false;
        }

        private bool HasPositionInDirection(Vector3Int center, Vector3Int direction)
        {
            var targetPosition = center + direction;
            
            return _positions.Contains(targetPosition);
        }
    }
}
