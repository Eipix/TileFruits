using System;
using UnityEngine;

namespace Generator
{
    [Serializable]
    public class TileMapGeneratorSettings
    {
        [field: SerializeField] public Vector2Int MapSize { get; private set; } = Vector2Int.one * 5;
        [field: SerializeField] public Vector3 Center { get; private set; }
        [field: SerializeField] public float PaddingBetweenTiles { get; private set; }
    }
}
