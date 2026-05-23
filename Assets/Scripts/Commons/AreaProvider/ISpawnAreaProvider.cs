using UnityEngine;

namespace Commons.AreaProvider
{
    public interface ISpawnAreaProvider
    {
        public ISpawnArea Current { get; }

        public Vector3 GetRandomPoint() => Current.GetRandomPoint();
        public Vector3 GetRandomPoint(System.Random random) => Current.GetRandomPoint(random);
    }
}
