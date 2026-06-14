using System;
using System.Collections.Generic;
using System.Linq;
using Commons.Extensions;
using Gameplay;
using Generator.DistributionStrategies.Base;
using URandom = UnityEngine.Random;

using static Constants.MahjongConstants;

namespace Generator.DistributionStrategies.Implementations.ReverseTraversal
{
    public class ReverseTraversalStrategy : DistributionStrategy<ReverseTraversalStrategyConfig>
    {
        private readonly Dictionary<int, List<Slot>> _slotsByLayers = new();

        public ReverseTraversalStrategy(ReverseTraversalStrategyConfig config, DistributionSettings settings) : base(config, settings)
        {
            int maxVirtualCapacity = config.MaxVirtualTrayCapacity;
            
            if (maxVirtualCapacity > TrayCapacity)
                throw new ArgumentOutOfRangeException($@"
Virtual capacity {maxVirtualCapacity} 
is more than maxTray capacity {TrayCapacity}", nameof(config.MaxVirtualTrayCapacity));
        }

        protected override void OnDistribute()
        {
            SortSlotsByLayer();
            var orderedSlots = GetOrderedSlots();
            var sequence = GenerateSolvableSequence(orderedSlots.Count);
            DistributeTiles(orderedSlots, sequence);
        }

        private void SortSlotsByLayer()
        {
            foreach (var (position, slot) in Map)
            {
                if (!_slotsByLayers.TryGetValue(position.z, out _))
                    _slotsByLayers[position.z] = new List<Slot>();
                    
                _slotsByLayers[position.z].Add(slot);
            }
        }

        private List<Slot> GetOrderedSlots()
        {
            var orderedSlots = new List<Slot>();
            var sortedLayers = _slotsByLayers.Keys.OrderByDescending(z => z).ToList();
            
            foreach (var layer in sortedLayers)
            {
                var layerSlots = _slotsByLayers[layer];
                layerSlots.ShuffleInPlace();
                orderedSlots.AddRange(layerSlots);
            }
            
            return orderedSlots;
        }

        private List<int> GenerateSolvableSequence(int totalSlots)
        {
            int numGroups = totalSlots / TilesPerMatch;
            List<int> sequence = new(totalSlots);
            Dictionary<int, int> groupCounts = new();

            List<int> unstartedGroups = Enumerable.Range(0, numGroups).ToList();
            List<int> activeGroups = new List<int>();
            int currentVirtualTraySize = 0;

            for (int i = 0; i < totalSlots; i++)
            {
                List<int> possibleChoices = new();

                int requiredTilesToFinish = activeGroups.Sum(g => TilesPerMatch - groupCounts[g]);
                int remainingSlots = totalSlots - i;

                if (currentVirtualTraySize < Config.MaxVirtualTrayCapacity &&
                    unstartedGroups.Count > 0 &&
                    remainingSlots >= requiredTilesToFinish + TilesPerMatch)
                {
                    possibleChoices.Add(-1);
                    possibleChoices.Add(-1);
                }

                if (activeGroups.Count > 0)
                    possibleChoices.AddRange(activeGroups);

                int random = URandom.Range(0, possibleChoices.Count);
                int choice = possibleChoices[random];

                if (choice is -1)
                {
                    int newGroup = unstartedGroups[0];
                    unstartedGroups.RemoveAt(0);

                    activeGroups.Add(newGroup);
                    groupCounts[newGroup] = 1;
                    sequence.Add(newGroup);
                    currentVirtualTraySize++;
                }
                else
                {
                    int groupId = choice;
                    groupCounts[groupId]++;
                    sequence.Add(groupId);

                    if (groupCounts[groupId] < TilesPerMatch)
                    {
                        currentVirtualTraySize++;
                    }
                    else
                    {
                        currentVirtualTraySize -= (TilesPerMatch - 1);
                        activeGroups.Remove(groupId);
                    }
                }
            }

            return sequence;
        }
        
        private void DistributeTiles(List<Slot> orderedSlots, List<int> sequence)
        {
            Dictionary<int, TileConfig> groupTileMap = new();

            for (int i = 0; i < orderedSlots.Count; i++)
            {
                int groupId = sequence[i];

                if (groupTileMap.ContainsKey(groupId) is false)
                    groupTileMap[groupId] = TileList.GetRandom();

                orderedSlots[i].Tile = groupTileMap[groupId];
            }
        }

        private void DistributeTiles(List<Slot> orderedSlots)
        {
            for (int i = 0; i < orderedSlots.Count; i += TilesPerMatch)
            {
                DistributePair(orderedSlots, i);
            }
        }

        private void DistributePair(List<Slot> orderedSlots, int i)
        {
            Slot slot1 = orderedSlots[i];
            Slot slot2 = orderedSlots[i + 1];
            Slot slot3 = orderedSlots[i + 2];

            var tileData = TileList.GetRandom();

            slot1.Tile = tileData;
            slot2.Tile = tileData;
            slot3.Tile = tileData;
        }
    }
}
