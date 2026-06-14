using Gameplay;
using Generator.DistributionStrategies.Base;

namespace Generator.DistributionStrategies.Implementations.SingleTile
{
    public class SingleTileStrategy : DistributionStrategy<SingleTileStrategyConfig>
    {
        public SingleTileStrategy(SingleTileStrategyConfig config, DistributionSettings settings) : base(config, settings)
        {
        }

        protected override void OnDistribute()
        {
            foreach (var slot in Map.Slots)
                slot.Tile = Config.TargetTile;
        }
    }
}
