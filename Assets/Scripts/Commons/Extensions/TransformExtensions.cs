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

        public static IReadOnlyList<T> GetComponentsInChildrenRecursively<T>(this Transform transform, bool includeSelf = true) where T : Component
        {
            var results = new List<T>();
            FindComponentsRecursively(transform, results, includeSelf);
            return results;
        }

        private static void FindComponentsRecursively<T>(Transform current, List<T> results, bool includeSelf = true) where T : Component
        {
            if (includeSelf && current.TryGetComponent(out T component))
                results.Add(component);

            foreach (Transform child in current)
            {
                FindComponentsRecursively(child, results, true);
            }
        }

        public static T GetComponentInChildrenRecursively<T>(this Transform transform) where T : Component
        {
            if (transform.TryGetComponent(out T component))
                return component;

            foreach (Transform child in transform)
            {
                T found = GetComponentInChildrenRecursively<T>(child);

                if (found is not null)
                    return found;
            }

            return default;
        }

        public static List<Transform> GetAllDescendants(this Transform root, bool includeSelf = true)
        {
            var result = new List<Transform>();

            if (includeSelf)
                result.Add(root);

            var stack = new Stack<Transform>();

            for (int i = root.childCount - 1; i >= 0; i--)
                stack.Push(root.GetChild(i));

            while (stack.Count > 0)
            {
                var current = stack.Pop();
                result.Add(current);

                for (int i = current.childCount - 1; i >= 0; i--)
                    stack.Push(current.GetChild(i));
            }

            return result;
        }
    }
}
