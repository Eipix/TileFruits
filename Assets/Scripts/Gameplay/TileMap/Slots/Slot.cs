using System;
using Gameplay;
using UnityEngine;

namespace Generator
{
    public class Slot : IReadOnlySlot, IDisposable
    {
        private TileConfig _tile;

        public TileConfig Tile
        {
            get => _tile;
            set
            {
                if (value == null)
                {
                    Debug.LogError("Can't set Tile to null!");
                    return;
                }
                
                if (_tile == null)
                    _tile = value;
                else
                    Debug.LogWarning("TileConfig is already set");
            }
        }
        
        public Vector3Int Position { get; }

        public bool IsEmpty => Tile == null;

        public Slot(Vector3Int position)
        {
            Position = position;
        }
        
        public void Clear() => Tile = null;

        public void Dispose()
        {
            _tile = null;
        }
    }
}
