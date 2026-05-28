using UnityEngine;

namespace Generator.GenerationStrategies.PyramidStrategy
{
    public class PyramidStrategy : IGenerationStrategy
    {
        public void GenerateShape(TileMap map)
        {
            var size = map.Size;
            int width = size.x;
            int height = size.y;
            
            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < height; j++)
                {
                    Vector3Int position = new(width, height, 0);
                    map.TryAddSlot(position);
                }
            }
        }
    }
}
