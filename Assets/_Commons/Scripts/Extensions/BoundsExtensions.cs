using System.Collections.Generic;
using UnityEngine;

namespace Commons.Extensions
{
    public static class BoundsExtensions
    {
        public static Bounds EncapsulateBounds(this IEnumerable<Collider> colliders)
        {
            var enumerator = colliders.GetEnumerator();

            if (enumerator.MoveNext() is false)
                return default;

            Bounds bounds = enumerator.Current.bounds;

            while (enumerator.MoveNext())
            {
                bounds.Encapsulate(enumerator.Current.bounds);
            }

            return bounds;
        }

        public static Bounds EncapsulateBounds(this IEnumerable<Renderer> renderers)
        {
            var enumerator = renderers.GetEnumerator();

            if (enumerator.MoveNext() is false)
                return default;

            Bounds bounds = enumerator.Current.bounds;

            while (enumerator.MoveNext())
            {
                bounds.Encapsulate(enumerator.Current.bounds);
            }

            return bounds;
        }

        public static Vector3 GetRightFace(this Bounds b)
        {
            return b.center + new Vector3(b.extents.x, 0, 0);
        }

        public static Vector3 GetLeftFace(this Bounds b)
        {
            return b.center - new Vector3(b.extents.x, 0, 0);
        }

        public static Vector3 GetTopFace(this Bounds b)
        {
            return b.center + new Vector3(0, b.extents.y, 0);
        }

        public static Vector3 GetBottomFace(this Bounds b)
        {
            return b.center - new Vector3(0, b.extents.y, 0);
        }

        public static Vector3 GetForwardFace(this Bounds b)
        {
            return b.center + new Vector3(0, 0, b.extents.z);
        }

        public static Vector3 GetBackFace(this Bounds b)
        {
            return b.center - new Vector3(0, 0, b.extents.z);
        }
    }
}
