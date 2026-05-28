using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Commons.Extensions
{
    public static class RandomExtensions
    {
        public static int GetSeed() => Random.Range(int.MinValue, int.MaxValue);

        public static T GetRandom<T>() where T : struct, Enum
        {
            string[] names = Enum.GetNames(typeof(T));
            int index = Random.Range(0, names.Length);
            return Enum.Parse<T>(names[index]);
        }

        public static Vector3 Range(Vector3 min, Vector3 max)
        {
            float x = Random.Range(min.x, max.x);
            float y = Random.Range(min.y, max.y);
            float z = Random.Range(min.z, max.z);

            return new(x, y, z);
        }

        public static Vector3 Range(Vector3 min, Vector3 max, System.Random random)
        {
            float x = (float)(min.x + (max.x - min.x) * random.NextDouble());
            float y = (float)(min.y + (max.y - min.y) * random.NextDouble());
            float z = (float)(min.z + (max.z - min.z) * random.NextDouble());

            return new(x, y, z);
        }

        public static T GetRandom<T>(this IEnumerable<T> ienumerable)
        {
            using var enumerator = ienumerable.GetEnumerator();

            if (enumerator.MoveNext() is false)
                throw new InvalidOperationException("Коллекция пуста");

            T result = enumerator.Current;
            int count = 1;

            while (enumerator.MoveNext())
            {
                count++;

                if (Random.Range(0, count) == 0)
                    result = enumerator.Current;
            }

            return result;
        }
    }
}
