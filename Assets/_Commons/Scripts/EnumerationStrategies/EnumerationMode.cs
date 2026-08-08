using System;
using System.Collections.Generic;
using System.Linq;
using _Commons.Scripts.EnumerationStrategies.Implementations;

namespace _Commons.Scripts.EnumerationStrategies
{
    public enum EnumerationMode
    {
        Linear,
        Random,
        Shuffle
    }
    
    public static class EnumerationModeExtensions
    {
        public static EnumerationStrategy<T> GetStrategy<T>(this EnumerationMode mode, IEnumerable<T> items) => mode switch
        {
            EnumerationMode.Linear => new LinearStrategy<T>(items.ToList()),
            EnumerationMode.Random => new RandomStrategy<T>(items.ToList()),
            EnumerationMode.Shuffle => new ShuffleStrategy<T>(items.ToList()),
            _ => throw new ArgumentOutOfRangeException(nameof(mode), mode, null)
        };
    }
}
