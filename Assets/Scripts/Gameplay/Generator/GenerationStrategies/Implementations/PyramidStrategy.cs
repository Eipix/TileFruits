using System;
using Generator.GenerationStrategies.Base;
using NaughtyAttributes;
using UnityEngine;

namespace Generator.GenerationStrategies.PyramidStrategy
{
    [Serializable]
    public class PyramidStrategy : GenerationStrategy
    {
        private const int TileGridSize = 2;
        
        [field: SerializeField] public bool UnlimitedLayers { get; private set; }
        
        [field: SerializeField, Min(1), HideIf(nameof(UnlimitedLayers))]
        public int MaxLayers { get; private set; } = 5;
        
        protected override void OnGenerateShape()
        {
            if (UnlimitedLayers)
                FillMap();

            for (int layer = 0; layer < MaxLayers; layer++)
            {
                if (TryCoverLayer(layer) is false)
                    break;
            }
        }

        private void FillMap()
        {
            int layer = 0;

            while (TryCoverLayer(layer))
                layer++;
        }

        private bool TryCoverLayer(int layer)
        {
            bool hasFreePositionInLayer = false;
            
            int endX = Size.x - layer;
            int endY = Size.y - layer;

            for (int x = layer; x < endX; x += TileGridSize)
            {
                for (int y = layer; y < endY; y += TileGridSize)
                {
                    Vector3Int position = new(x, y, layer);
                    
                    if(Map.TryAdd(position))
                        hasFreePositionInLayer = true;
                }
            }

            return hasFreePositionInLayer;
        }
    }
}
