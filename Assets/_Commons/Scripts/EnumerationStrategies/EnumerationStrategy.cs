using System;
using System.Collections.Generic;

namespace _Commons.Scripts.EnumerationStrategies
{
    public abstract class EnumerationStrategy<T>
    {
        protected List<T> Items { get; }

        protected EnumerationStrategy(List<T> items)
        {
            if (items.Count < 1)
                throw new InvalidOperationException("Items must have at least one item");
            
            Items = items;
        }

        public abstract T Next();
        
        public virtual void Reset() { }
    }
}
