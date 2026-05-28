using Commons.Extensions;
using Gameplay;

namespace Generator.DistributionStrategies.Implementations.Random
{
    public class RandomStrategy : DistributionStrategy<RandomSrategyConfig>
    {
        public RandomStrategy(RandomSrategyConfig config, TileMap map, TileList tileList)
            : base(config, map, tileList) { }

        protected override void OnDistribute()
        {
            foreach (var slot in Map.Slots)
                slot.Tile = TileList.GetRandom();
        }
    }
}
