using System;
using UnityEngine;

namespace Generator
{
    [Serializable]
    public class GeneratorSettings
    {
        public const int MinMapSize = 3;
        
        [field: SerializeField] public Vector2Int MapSize { get; private set; } = Vector2Int.one * 10;
        [field: SerializeField] public Vector3 Center { get; private set; }
        [field: SerializeField] public float PaddingBetweenTiles { get; private set; }

        public void OnValidate()
        {
            if (MapSize.x < 1 || MapSize.y < 1)
                MapSize = Vector2Int.one * MinMapSize;
        }
    }
}
