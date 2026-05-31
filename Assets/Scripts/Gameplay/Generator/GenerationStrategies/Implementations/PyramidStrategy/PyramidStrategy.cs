using Gameplay.Generator.GenerationStrategies.Base.Strategy;
using UnityEngine;

namespace Generator.GenerationStrategies.PyramidStrategy
{
    public class PyramidStrategy : GenerationStrategy<PyramidStrategyConfig>
    {
        private const int TileGridSize = 2;
        
        public PyramidStrategy(PyramidStrategyConfig config, TileMap map) : base(config, map) { }

        protected override void OnGenerateShape()
        {
            if (Config.UnlimitedLayers)
                FillMap();
            
            int maxLayers = Config.MaxLayers;

            for (int layer = 0; layer < maxLayers; layer++)
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
