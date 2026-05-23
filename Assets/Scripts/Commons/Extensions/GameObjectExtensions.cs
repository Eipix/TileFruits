using System;
using System.Collections.Generic;
using UnityEngine;

namespace Commons.Extensions
{
    public static class GameObjectExtensions
    {
        public static bool HasAnyComponent(this GameObject gameObject, IEnumerable<Type> types)
        {
            if (gameObject is null || types is null)
                return false;

            foreach (var type in types)
            {
                if (gameObject.TryGetComponent(type, out _))
                {
                    return true;
                }
            }

            return false;
        }

        public static bool HasAnyComponent(this GameObject gameObject, params Type[] types)
        {
            if (gameObject is null || types is null || types.Length is 0)
                return false;

            for (int i = 0; i < types.Length; i++)
            {
                if (gameObject.TryGetComponent(types[i], out _))
                {
                    return true;
                }
            }
            return false;
        }
    }
}
