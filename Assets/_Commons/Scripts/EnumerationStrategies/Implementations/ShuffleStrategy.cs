using System.Collections.Generic;
using Commons.Extensions;

namespace _Commons.Scripts.EnumerationStrategies.Implementations
{
    public class ShuffleStrategy<T> : EnumerationStrategy<T>
    {
        private int? _seed;
        private int _index = int.MaxValue;

        public ShuffleStrategy(List<T> items) : base(items) { }

        public ShuffleStrategy(List<T> items, int seed) : base(items)
        {
            _seed = seed;
        }

        public override T Next()
        {
            if (_index >= Items.Count)
                Reset();
            
            return Items[_index++];
        }

        public override void Reset()
        {
            _index = 0;
            
            if (_seed.HasValue)
                Items.SeamlessShuffle(_seed.Value);
            else
                Items.SeamlessShuffle();
        }
    }
}
