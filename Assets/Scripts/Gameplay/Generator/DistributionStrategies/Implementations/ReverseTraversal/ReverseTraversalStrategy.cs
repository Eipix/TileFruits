using System.Collections.Generic;
using System.Linq;
using Commons.Extensions;
using Gameplay;

namespace Generator.DistributionStrategies.Implementations.ReverseTraversal
{
    public class ReverseTraversalStrategy : DistributionStrategy<ReverseTraversalStrategyConfig>
    {
        private readonly Dictionary<int, List<Slot>> _slotsByLayers = new();
        
        public ReverseTraversalStrategy(ReverseTraversalStrategyConfig config, TileMap map, TileList tileList)
            : base(config, map, tileList) { }

        public override void Distribute()
        {
            SortSlotsByLayer();
            var orderedSlots = GetOrderedSlots();
            DistributeTiles(orderedSlots);
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

        private void DistributeTiles(List<Slot> orderedSlots)
        {
            for (int i = 0; i < orderedSlots.Count; i += 2)
            {
                DistributePair(orderedSlots, i);
            }
        }

        private void DistributePair(List<Slot> orderedSlots, int i)
        {
            Slot slot1 = orderedSlots[i];
            Slot slot2 = orderedSlots[i + 1];

            var tileData = TileList.GetRandom(); 

            slot1.Tile = tileData;
            slot2.Tile = tileData;
        }
    }
}
