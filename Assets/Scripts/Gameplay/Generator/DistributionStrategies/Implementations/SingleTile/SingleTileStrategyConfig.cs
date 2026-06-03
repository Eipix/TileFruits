using Gameplay;
using Generator.DistributionStrategies.Base.Config;
using NaughtyAttributes;
using UnityEngine;

namespace Generator.DistributionStrategies.Implementations.SingleTile
{
    [CreateAssetMenu(menuName = "Generator/DistributionStrategies/SingleTile")]
    public class SingleTileStrategyConfig : DistributionStrategyConfig<SingleTileStrategy>
    {
        [field: SerializeField, Required]
        public TileConfig TargetTile { get; private set; } 
    }
}
