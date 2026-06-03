using System;
using System.Collections.Generic;
using UnityEngine;

namespace Generator
{
    public interface ITileMap : IEnumerable<KeyValuePair<Vector3Int, IReadOnlySlot>>
    {
        event Action<Vector3Int> TileTaken;
        
        Vector2Int Size { get; }
        int HighestLayer { get; }
        int Count { get; }

        bool Contains(Vector3Int position);
        bool CanTakeTile(Vector3Int position);
        bool TryTakeTile(Vector3Int position);
        bool IsBlockedByAbove(Vector3Int position);
        void Dispose();
    }
}
