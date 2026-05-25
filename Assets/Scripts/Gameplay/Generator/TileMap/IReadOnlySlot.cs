using Gameplay;
using UnityEngine;

namespace Generator
{
    public interface IReadOnlySlot
    {
        public TileConfig Tile { get; }
        
        public Vector3Int Position { get; }

        public bool IsEmpty => Tile == null;
    }
}
