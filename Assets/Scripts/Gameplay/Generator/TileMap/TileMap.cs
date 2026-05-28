using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Generator
{
    public class TileMap : ITileMap, IEnumerable<KeyValuePair<Vector3Int, Slot>>
    {
        private readonly Dictionary<Vector3Int, Slot> _slots = new();
        
        private readonly (int x, int y)[] _positionsAround =
        {
            (0, 1), (0, -1),
            (1, 0), (-1, 0),
            (-1, 1), (1, 1),
            (-1, -1), (1, -1)
        };

        public IEnumerable<Vector3Int> Positions => _slots.Keys;
        public IEnumerable<Slot> Slots => _slots.Values;
        
        public Vector2Int Size { get; }
        public int HighestLayer { get; private set; }
        public int Count => _slots.Count;
        
        public TileMap(Vector2Int size) => Size = size;
        
        public void CoverLayer(int layer, out int slotsAdded)
        {
            slotsAdded = 0;
            
            int endX = Size.x;
            int endY = Size.y;
            
            for (int x = 0; x < endX; x++)
            {
                for (int y = 0; y < endY; y++)
                {
                    Vector3Int position = new(x, y, layer);
                    
                    if(TryAdd(position))
                        slotsAdded++;
                }
            }
        }

        public bool Remove(Vector3Int position) => _slots.Remove(position);

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

        public bool CanTakeBone(Vector3Int position)
        {
            throw new NotImplementedException();
        }

        public void Clear()
        {
            foreach (var slot in _slots.Values)
                slot.Dispose();

            _slots.Clear();
        }
        
        public bool HasFreePosition(int layer)
        {
            if(layer < 0)
                throw new ArgumentOutOfRangeException(nameof(layer));
            
            for (int x = 0; x < Size.x; x++)
            {
                for (int y = 0; y < Size.y; y++)
                {
                    Vector3Int testPosition = new(x, y, layer);
            
                    if (IsValidPosition(testPosition, out _))
                        return true;
                }
            }
    
            return false;
        }

        private bool IsValidPosition(Vector3Int position, out string errorMessage)
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
            
            if(HasNeighbour(position))
            {
                errorMessage = $"Can't add a slot to a position {position}";
                return false;
            }
            
            if (_slots.TryGetValue(position, out var slot) && slot != null)
            {
                errorMessage = $"Slot in position ({position}) has already been added";
                return false;
            }
            
            errorMessage = null;
            return true;
        }

        private bool HasNeighbour(Vector3Int slotPosition)
        {
            var position = new Vector3Int(slotPosition.x, slotPosition.y, slotPosition.z);

            foreach (var (xOffset, yOffset) in _positionsAround)
            {
                position.Set(slotPosition.x + xOffset, slotPosition.y + yOffset, slotPosition.z);
                return _slots.TryGetValue(position, out var slot) && slot != null;
            }

            return false;
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
    }
}
