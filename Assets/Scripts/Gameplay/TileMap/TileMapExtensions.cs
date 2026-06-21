using System;
using System.Collections.Generic;
using System.Linq;
using Commons.Utils;
using UnityEngine;

namespace Generator
{
    public static class TileMapExtensions
    {
        public static int GetLowestValidLayer(this TileMap tileMap, Vector2Int position, int supportRadius = 1)
        {
            for (int z = 0; ; z++)
            {
                int cellX = position.x;
                int cellY = position.y;
                
                bool isOccupied = tileMap.Positions.Any(pos => 
                    pos.z == z && 
                    Mathf.Abs(pos.x - cellX) <= 1 && 
                    Mathf.Abs(pos.y - cellY) <= 1);
                
                if (isOccupied)
                    continue;

                if (z == 0)
                    return 0;

                bool hasSupport = tileMap.Positions.Any(pos =>
                    pos.z == z - 1 &&
                    Mathf.Abs(pos.x - cellX) <= supportRadius &&
                    Mathf.Abs(pos.y - cellY) <= supportRadius);

                if (hasSupport)
                    return z;
            }
        }
        
        public static int GetHighestLayer(this TileMap tileMap, Vector2Int position)
        {
            int highestLayer = tileMap.HighestLayer;
            Vector3Int position3d = (Vector3Int)position;

            for (int i = highestLayer; i >= 0; i--)
            {
                position3d.z = i;

                if (tileMap.TryGet(position3d, out _))
                    return i;
            }
            
            return 0;
        }

        public static int GetHighestLayerAround(this TileMap tileMap, Vector2Int position)
        {
            int currentHighestLayer = tileMap.GetHighestLayer(position);
            
            if (currentHighestLayer == tileMap.HighestLayer) 
                return currentHighestLayer;

            foreach (Vector3Int dir3 in TileMapUtils.DirectionsAround)
            {
                Vector2Int direction = position + (Vector2Int)dir3;
                int highestLayer = tileMap.GetHighestLayer(direction);

                if (highestLayer > currentHighestLayer)
                {
                    currentHighestLayer = highestLayer;
    
                    if (currentHighestLayer == tileMap.HighestLayer) 
                        break; 
                }
            }

            return currentHighestLayer;
        }
        
        public static bool HasFreePositionOnLayer(this TileMap tileMap, int layer)
        {
            if(layer < 0)
                throw new ArgumentOutOfRangeException(nameof(layer));
            
            for (int x = 0; x < tileMap.Size.x; x++)
            {
                for (int y = 0; y < tileMap.Size.y; y++)
                {
                    Vector3Int testPosition = new(x, y, layer);
            
                    if (tileMap.IsValidPosition(testPosition, out _))
                        return true;
                }
            }
    
            return false;
        }
        
        
        public static List<Vector3Int> GetPositionsFromLayer(this TileMap tileMap, int layer)
        {
            List<Vector3Int> positions = new(tileMap.Count);
            
            foreach (var position in tileMap.Positions)
            {
                if(position.z == layer)
                    positions.Add(position);
            }

            return positions;
        }
        
        public static void CoverLayer(this TileMap tileMap, int layer, out int slotsAdded)
        {
            slotsAdded = 0;
            
            int endX = tileMap.Size.x;
            int endY = tileMap.Size.y;
            
            for (int x = 0; x < endX; x++)
            {
                for (int y = 0; y < endY; y++)
                {
                    Vector3Int position = new(x, y, layer);
                    
                    if(tileMap.TryAdd(position))
                        slotsAdded++;
                }
            }
        }
    }
}
