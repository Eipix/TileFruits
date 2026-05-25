using UnityEngine;

namespace Commons.AreaProvider
{
    public interface ISpawnArea
    {
        public Vector3 GetRandomPoint();
        public Vector3 GetRandomPoint(System.Random random);
    }
}
