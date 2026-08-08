using System;
using System.Collections.Generic;
using Random = UnityEngine.Random;

namespace Commons.Extensions
{
    public static class ListExtensions
    {
        public static void SeamlessShuffle<T>(this List<T> list, int seed) =>
            SeamlessShuffle(list, () => list.ShuffleInPlace(seed));

        public static void SeamlessShuffle<T>(this List<T> list) =>
            SeamlessShuffle(list, list.ShuffleInPlace);
        
        private static void SeamlessShuffle<T>(this List<T> list, Action action)
        {
            if (list == null || list.Count <= 1)
                return;

            T lastItem = list[^1];
            action.Invoke();

            if (EqualityComparer<T>.Default.Equals(list[0], lastItem))
            {
                int swapIndex = Random.Range(1, list.Count);
                (list[0], list[swapIndex]) = (list[swapIndex], list[0]);
            }
        }
        
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
