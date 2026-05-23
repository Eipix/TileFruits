using System;
using Commons.Extensions;
using NaughtyAttributes;
using UnityEngine;

namespace Commons.AreaProvider.SpawnArea
{
    public abstract class SpawnArea : MonoBehaviour, ISpawnArea
    {
        [field: SerializeField, Min(0f)] public float Priority { get; private set; }

        [Button]
        private void AutoCalculatePriority()
        {
            Awake();
            Priority = CalculatePriority();
        }

        protected virtual void Awake() { }

        public Vector3 GetRandomPoint()
        {
            var area = GetArea();
            return RandomExtensions.Range(area.min, area.max);
        }

        public Vector3 GetRandomPoint(System.Random random)
        {
            var area = GetArea();
            return RandomExtensions.Range(area.min, area.max, random);
        }

        protected abstract Bounds GetArea();

        protected virtual float CalculatePriority()
        {
            Bounds area = GetArea();
            float volume = area.size.x * area.size.z;
            float scaleFactor = transform.lossyScale.x * transform.lossyScale.z;

            return Mathf.Sqrt(volume * scaleFactor);
        }
    }
}
