using Gameplay.Generator.GenerationStrategies.Base.Strategy;
using UnityEngine;

namespace Generator.GenerationStrategies.Implementations.Custom
{
    public class CustomStrategy : GenerationStrategy<CustomStrategyConfig>
    {
        public CustomStrategy(CustomStrategyConfig config, Vector2Int size) : base(config, size) { }
        
        protected override void OnGenerateShape()
        {
            foreach (var position in Config.Positions)
                Map.Add(position);
        }
    }
}
