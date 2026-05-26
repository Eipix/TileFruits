using System.Collections.Generic;
using UnityEngine;

namespace Commons.Extensions
{
    public static class ListExtensions
    {
        public static void ShuffleInPlace<T>(this List<T> list)
        {
            for (int i = list.Count - 1; i > 0; i--)
            {
                int j = Random.Range(0, i + 1);
                (list[i], list[j]) = (list[j], list[i]);
            }
        }

        public static void ShuffleInPlace<T>(this List<T> list, int seed)
        {
            var random = new System.Random(seed);

            for (int i = list.Count - 1; i > 0; i--)
            {
                int j = random.Next(0, i + 1);
                (list[i], list[j]) = (list[j], list[i]);
            }
        }

        public static void ShuffleInPlace<T>(this List<T> list, System.Random random)
        {
            for (int i = list.Count - 1; i > 0; i--)
            {
                int j = random.Next(0, i + 1);
                (list[i], list[j]) = (list[j], list[i]);
            }
        }

        public static T GetRandom<T>(this IReadOnlyList<T> list, System.Random random)
        {
            return list[random.Next(0, list.Count)];
        }

    }
}
