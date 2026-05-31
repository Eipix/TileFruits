using Gameplay;

namespace Generator.DistributionStrategies.Implementations.SingleTile
{
    public class SingleTileStrategy : DistributionStrategy<SingleTileStrategyConfig>
    {
        public SingleTileStrategy(SingleTileStrategyConfig config, TileMap map, TileList tileList) : base(config, map, tileList)
        {
        }

        protected override void OnDistribute()
        {
            foreach (var slot in Map.Slots)
                slot.Tile = Config.TargetTile;
        }
    }
}
