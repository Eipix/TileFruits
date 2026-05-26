using System.Collections.Generic;
using UnityEngine;

namespace Generator
{
    public interface ITileMap : IEnumerable<KeyValuePair<Vector3Int, IReadOnlySlot>>
    {
        Vector2Int Size { get; }
        int HighestLayer { get; }
        int Count { get; }
        
        bool CanTakeBone(Vector3Int position);
    }
}
