using Gameplay.Generator.GenerationStrategies.Base.Strategy;
using UnityEngine;

namespace Generator.GenerationStrategies.Implementations.Fullfill
{
    public class FullfillStrategy : GenerationStrategy<FullfillStrategyConfig>
    {
        public FullfillStrategy(FullfillStrategyConfig config, Vector2Int size) : base(config, size)
        {
        }

        protected override void OnGenerateShape()
        {
            int layers = Config.Layers;
            
            for (int i = 0; i < layers; i++)
            {
                Map.CoverLayer(i, out _);
            }
        }
    }
}
