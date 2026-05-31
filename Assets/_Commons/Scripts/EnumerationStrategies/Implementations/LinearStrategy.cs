using System.Collections.Generic;

namespace _Commons.Scripts.EnumerationStrategies.Implementations
{
    public class LinearStrategy<T> : EnumerationStrategy<T>
    {
        private int _index;
        
        public LinearStrategy(List<T> items) : base(items) { }

        public override T Next()
        {
            if (_index >= Items.Count)
                Reset();
            
            return Items[_index++];
        }

        public override void Reset() => _index = 0;
    }
}
