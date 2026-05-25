using Commons.Extensions;
using UnityEngine;

namespace Commons.AreaProvider.SpawnArea
{
    public class BoxSpawnArea : SpawnArea
    {
        [SerializeField] private bool _calculateByPeaks = true;

        private BoxCollider[] _colliders;

        protected override void Awake()
        {
            _colliders = GetComponentsInChildren<BoxCollider>();
        }

        protected override Bounds GetArea()
        {
            var bounds = _colliders.EncapsulateBounds();
            var min = bounds.min;
            var max = bounds.max;

            if(_calculateByPeaks)
            {
                min.y = max.y;
                bounds.SetMinMax(min, max);
            }

            return bounds;
        }
    }
}
