using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

namespace Generator
{
    public class TileMap : IEnumerable<KeyValuePair<Vector3Int, IReadOnlySlot>>
    {
        private readonly Dictionary<Vector3Int, Slot> _slots = new();
        
        public IEnumerable<KeyValuePair<Vector3Int, IReadOnlySlot>> Slots
            => (IEnumerable<KeyValuePair<Vector3Int, IReadOnlySlot>>)_slots;
        
        public Vector2Int Size { get; }
        
        public TileMap(Vector2Int size)
        {
            Size = size;
        }
        
        public IReadOnlySlot this[Vector3Int position]
        {
            get
            {
                var slot = _slots[position];
                
                if(slot == null)
                    throw new InvalidOperationException("Slot not found in position " + position);
                
                return slot;
            }
        }

        public bool TryAddSlot(Vector3Int position)
        {
            if(position.x < 0 || position.y < 0)
            {
                Debug.LogError($"Position cannot be negative ({position})");
                return false;
            }
            
            if (position.x > Size.x || position.y > Size.y)
            {
                Debug.LogError($"Slot position ({position}) out of range ({Size})");
                return false;
            }
            
            if(HasNeighbourSlot(position))
                return false;
            
            if (_slots.TryGetValue(position, out var slot) && slot != null)
            {
                Debug.LogError($"Slot in position ({position}) has already been added");
                return false;
            }
            
            _slots[position] = new Slot(position);
            return true;
        }

        public bool TryGetSlot(Vector3Int position, out Slot slot)
        {
            return _slots.TryGetValue(position, out slot);
        }

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

        private bool HasNeighbourSlot(Vector3Int slotPosition)
        {
            var position = new Vector3Int(slotPosition.x, slotPosition.y, slotPosition.z);

            return HasSlot(0, 1)
                   || HasSlot(0, -1)
                   || HasSlot(1, 0)
                   || HasSlot(-1, 0);

            bool HasSlot(int xOffset, int yOffset)
            {
                position.Set(slotPosition.x + xOffset, slotPosition.y + yOffset, slotPosition.z);
                return _slots.TryGetValue(position, out var slot) && slot != null;
            }
        }

        public IEnumerator<KeyValuePair<Vector3Int, IReadOnlySlot>> GetEnumerator()
            => Slots.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}
