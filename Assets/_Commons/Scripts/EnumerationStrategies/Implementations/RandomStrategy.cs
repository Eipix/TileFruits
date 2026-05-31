using System;
using System.Collections.Generic;

namespace _Commons.Scripts.EnumerationStrategies.Implementations
{
    public class RandomStrategy<T> : EnumerationStrategy<T>
    {
        private readonly Random _random;

        public RandomStrategy(List<T> items, int seed) : base(items) =>
            _random = new(seed);

        public RandomStrategy(List<T> items) : base(items) =>
            _random = new();

        public override T Next() => Items[_random.Next(0, Items.Count)];
    }
}
