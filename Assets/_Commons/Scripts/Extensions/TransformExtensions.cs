using System.Collections.Generic;
using UnityEngine;

namespace Commons.Extensions
{
    public static class TransformExtensions
    {
        public static Bounds EncapsulateBoundsFromChildren(this Transform transform, bool includeSelf = false)
        {
            Bounds bounds;
            if (includeSelf)
            {
                var renderer = transform.GetComponent<Renderer>();
                bounds = renderer is null ? new() : renderer.bounds;
            }
            else
            {
                bounds = new();
            }

            int childCount = transform.childCount;

            for (int i = 0; i < childCount; i++)
            {
                var child = transform.GetChild(i);

                if(child.TryGetComponent(out Renderer renderer))
                    bounds.Encapsulate(renderer.bounds);
            }

            return bounds;
        }
    }
}
