using Generator.DistributionStrategies.Base;
using Generator.DistributionStrategies.Base.Config;

namespace Generator.DistributionStrategies
{
    public abstract class DistributionStrategy<T> : DistributionStrategyBase
        where T : DistributionStrategyConfigBase
    {
        protected new T Config { get; }

        public DistributionStrategy(T config, DistributionSettings settings) : base(config, settings)
        {
            Config = config;
        }
    }
}
