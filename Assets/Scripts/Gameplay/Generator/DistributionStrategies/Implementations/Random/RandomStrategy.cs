using System.Collections.Generic;
using Commons.Extensions;
using Constants;
using Gameplay;
using Generator.DistributionStrategies.Base;

namespace Generator.DistributionStrategies.Implementations.Random
{
    public class RandomStrategy : DistributionStrategy<RandomSrategyConfig>
    {
        public RandomStrategy(RandomSrategyConfig config, DistributionSettings settings) : base(config, settings)
        {
        }

        protected override void OnDistribute()
        {
            var tilesPool = GetTilesPool();
            
            tilesPool.ShuffleInPlace();

            int index = 0;
            foreach (var slot in Map.Slots)
            {
                slot.Tile = tilesPool[index];
                index++;
            }
        }

        private List<TileConfig> GetTilesPool()
        {
            int totalSlots = Map.Count;
            int tilesRequired = MahjongConstants.TilesPerMatch;

            List<TileConfig> tilesPool = new(totalSlots);

            int generatedCount = 0;
            while (generatedCount < totalSlots)
            {
                var config = TileList.GetRandom();

                for (int i = 0; i < tilesRequired && generatedCount < totalSlots; i++)
                {
                    tilesPool.Add(config);
                    generatedCount++;
                }
            }

            return tilesPool;
        }
    }
}
