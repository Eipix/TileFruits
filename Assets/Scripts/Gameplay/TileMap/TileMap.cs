using System;
using System.Collections;
using System.Collections.Generic;
using Commons.Utils;
using UnityEngine;

namespace Generator
{
    public class TileMap : ITileMap, IEnumerable<KeyValuePair<Vector3Int, Slot>>, IDisposable
    {
        private readonly Dictionary<Vector3Int, Slot> _slots = new();
        
        public event Action<Vector3Int> TileTaken;

        public IEnumerable<Vector3Int> Positions => _slots.Keys;
        public IEnumerable<Slot> Slots => _slots.Values;
        
        public Vector2Int Size { get; }
        public int HighestLayer { get; private set; }
        public int Count => _slots.Count;
        
        public TileMap(Vector2Int size) => Size = size;

        public bool Remove(Vector3Int position)
        {
            return _slots.Remove(position);
        }

        public bool TryAdd(Vector3Int position)
        {
            if(IsValidPosition(position, out _))
            {
                SetLayerIfHigher(position);
                _slots[position] = new Slot(position);
                return true;
            }

            return false;
        }

        public void AddRange(IEnumerable<Vector3Int> positions)
        {
            foreach (var position in positions)
                Add(position);
        }
        
        public void Add(Vector3Int position)
        {
            if (IsValidPosition(position, out var errorMessage) is false)
            {
                Debug.LogError(errorMessage);
                return;
            }
            
            SetLayerIfHigher(position);
            _slots[position] = new Slot(position);
        }

        public bool TryGet(Vector3Int position, out Slot slot)
            => _slots.TryGetValue(position, out slot);

        public bool Contains(Vector3Int position) => _slots.ContainsKey(position);

        public bool TryTakeTile(Vector3Int position)
        {
            if(CanTakeTile(position))
            {
                Remove(position);
                TileTaken?.Invoke(position);
                return true;
            }
            
            return false;
        }

        public bool CanTakeTile(Vector3Int position)
        {
            return IsBlockedByAbove(position) is false;
            
            /*if(_slots.ContainsKey(position) is false)
                return false;
            
            if (HasSlotInDirection(position, TileMapUtils.Left)
                && HasSlotInDirection(position, TileMapUtils.Right))
                return false;

            if (IsBlockedByAboveInternal(position))
                return false;
            
            return true;*/
        }

        public bool IsBlockedByAbove(Vector3Int position)
        {
            if(_slots.ContainsKey(position) is false)
                return true;
            
            return IsBlockedByAboveInternal(position);
        }

        public bool IsBlockedByAboveInternal(Vector3Int position)
        {
            if (HasPositionInDirection(position, TileMapUtils.UpLayer))
                return true;
            
            if (HasPositionInDirections(position, TileMapUtils.UpperDirectionsAround))
                return true;

            return false;
        }

        public void Clear()
        {
            foreach (var slot in _slots.Values)
                slot.Dispose();

            _slots.Clear();
        }

        public bool IsValidPosition(Vector3Int position) => IsValidPosition(position, out _);
        
        public bool IsValidPosition(Vector3Int position, out string errorMessage)
        {
            if(position.x < 0 || position.y < 0)
            {
                errorMessage = $"Position cannot be negative ({position})";
                return false;
            }
            
            if (position.x > Size.x || position.y > Size.y)
            {
                errorMessage = $"Slot position ({position}) out of range ({Size})";
                return false;
            }
            
            if(HasPositionInDirections(position, TileMapUtils.DirectionsAround))
            {
                errorMessage = $"Can't add a slot to a position {position}";
                return false;
            }
            
            if (_slots.TryGetValue(position, out var slot) && slot != null)
            {
                errorMessage = $"Slot in position ({position}) has already been added";
                return false;
            }

            if (position.z > 0 && HasPositionInDirection(position, TileMapUtils.DownLayer) is false
                && HasPositionInDirections(position, TileMapUtils.LowerDirectionsAround) is false)
            {
                errorMessage = $"Has no support under position {position}";
                return false;
            }
            
            errorMessage = null;
            return true;
        }
        
        private bool HasPositionInDirections(Vector3Int slotPosition, ReadOnlySpan<Vector3Int> directions)
        {
            foreach (var direction in directions)
            {
                if(HasPositionInDirection(slotPosition, direction))
                    return true;
            }

            return false;
        }

        private bool HasPositionInDirection(Vector3Int slotPosition, Vector3Int direction)
        {
            return _slots.ContainsKey(slotPosition + direction);
        }
        
        private void SetLayerIfHigher(Vector3Int position)
        {
            if(position.z > HighestLayer)
                HighestLayer = position.z;
        }

        IEnumerator<KeyValuePair<Vector3Int, IReadOnlySlot>> IEnumerable<KeyValuePair<Vector3Int, IReadOnlySlot>>.GetEnumerator()
        {
            foreach (var (position, slot) in _slots)
                yield return new(position, slot);
        }

        public IEnumerator<KeyValuePair<Vector3Int, Slot>> GetEnumerator() => _slots.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        public void Dispose()
        {
            Clear();
            TileTaken = null;
        }
    }
}
