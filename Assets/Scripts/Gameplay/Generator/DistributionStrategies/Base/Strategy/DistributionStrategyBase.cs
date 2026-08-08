using System;
using Gameplay;
using Generator.DistributionStrategies.Base;
using Generator.DistributionStrategies.Base.Config;

namespace Generator.DistributionStrategies
{
    public abstract class DistributionStrategyBase : IDistributionStrategy
    {
        protected DistributionStrategyConfigBase Config { get; }
        protected TileMap Map { get; }
        protected TileList TileList { get; }
        protected int TrayCapacity { get; }
        
        public DistributionStrategyBase(DistributionStrategyConfigBase config,
            DistributionSettings settings)
        {
            Config = config;
            Map = settings.TileMap;
            TileList = settings.TileList;
            TrayCapacity = settings.TraySettings.Capacity;
        }

        public void Distribute()
        {
            OnDistribute();
            
            foreach (var slot in Map.Slots)
            {
                if(slot.IsEmpty)
                    throw new InvalidOperationException("Not all slots are distributed");
            }
        }
        
        protected abstract void OnDistribute();
    }
}
