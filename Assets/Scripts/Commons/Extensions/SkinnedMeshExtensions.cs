using JetBrains.Annotations;
using UnityEngine;

namespace Commons.Extensions
{
    public static class SkinnedMeshExtensions
    {
        [CanBeNull]
        public static Transform GetClosestBone(this SkinnedMeshRenderer smr, Vector3 hitPosition)
        {
            Transform closestBone = null;
            float minDistanceSqr = float.MaxValue;

            foreach (var bone in smr.bones)
            {
                if (bone == null)
                {
                    continue;
                }

                float distSqr = (bone.position - hitPosition).sqrMagnitude;
                if (distSqr < minDistanceSqr)
                {
                    minDistanceSqr = distSqr;
                    closestBone = bone;
                }
            }

            return closestBone;
        }
    }
}
