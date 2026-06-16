using System;
using UnityEngine;

namespace Commons.Utils
{
    public static class TileMapUtils
    {
        public const int TileSize = 2;
        
        private static readonly Vector3Int[] _crossDirections =
        {
            Vector3Int.left, Vector3Int.right,
            Vector3Int.up, Vector3Int.down,
        };
        
        private static readonly Vector3Int[] _directionsAround =
        {
            Vector3Int.left, Vector3Int.right,
            Vector3Int.up, Vector3Int.down,
            new(-1, 1), new(1, 1),
            new(-1, -1), new(1, -1)
        };
        
        private static readonly Vector3Int[] _upperDirectionsAround =
        {
            new(0, 1, 1), new(0, -1, 1),
            new(1, 0, 1), new(-1, 0, 1),
            
            new(-1, 1, 1), new(1, 1, 1),
            new(-1, -1, 1), new(1, -1, 1)
        };
        
        private static readonly Vector3Int[] _lowerDirectionsAround =
        {
            new(0, 1, -1), new(0, -1, -1),
            new(1, 0, -1), new(-1, 0, -1),
            
            new(-1, 1, -1), new(1, 1, -1),
            new(-1, -1, -1), new(1, -1, -1)
        };
        
        public static ReadOnlySpan<Vector3Int> CrossDirections => _crossDirections;
        public static ReadOnlySpan<Vector3Int> DirectionsAround => _directionsAround;
        public static ReadOnlySpan<Vector3Int> UpperDirectionsAround => _upperDirectionsAround;
        public static ReadOnlySpan<Vector3Int> LowerDirectionsAround => _lowerDirectionsAround;
        
        public static Vector3Int Left => Vector3Int.left * TileSize;
        public static Vector3Int Right => Vector3Int.right * TileSize;
        
        public static Vector3Int UpLayer => Vector3Int.forward;
        public static Vector3Int DownLayer => Vector3Int.back;
    }
}
