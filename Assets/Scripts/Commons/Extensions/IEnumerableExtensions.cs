using System;
using System.Collections.Generic;
using System.Linq;

namespace Commons.Extensions
{
    public static class IEnumerableExtensions
    {
        public static IEnumerable<T> Shuffle<T>(this IEnumerable<T> ienumerable)
        {
            return ienumerable.OrderBy(x => UnityEngine.Random.value);
        }

        public static void ForEach<T>(this IEnumerable<T> objects, Action<T> action)
        {
            foreach (var obj in objects)
            {
                action.Invoke(obj);
            }
        }
    }
}
