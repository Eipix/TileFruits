using Generator.DistributionStrategies.Base.Config;
using UnityEngine;
using static Constants.MahjongConstants;

namespace Generator.DistributionStrategies.Implementations.ReverseTraversal
{
    [CreateAssetMenu(menuName = "Generator/DistributionStrategies/ReverseTraversal")]
    public class ReverseTraversalStrategyConfig : DistributionStrategyConfig<ReverseTraversalStrategy>
    {
        [field: SerializeField, Min(1)]
        public int MaxVirtualTrayCapacity { get; private set; } = TilesPerMatch;
        
    }
}
