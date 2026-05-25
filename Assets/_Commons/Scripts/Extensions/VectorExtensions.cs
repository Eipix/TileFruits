using System;
using UnityEngine;

namespace Commons.Extensions
{
    public static class VectorExtensions
    {
        public static Vector3 ToWorldDirection(this Vector2Int direction) => direction switch
        {
            _ when direction == Vector2Int.up => Vector3.forward,
            _ when direction == Vector2Int.down => Vector3.back,
            _ when direction == Vector2Int.left => Vector3.left,
            _ when direction == Vector2Int.right => Vector3.right,
            _ => throw new InvalidOperationException("Invalid direction")
        };

        public static Vector3 DividedBy(this Vector3 a, Vector3 b)
        {
            return new(
                DivideOrZero(a.x, b.x),
                DivideOrZero(a.y, b.y),
                DivideOrZero(a.z, b.z)
            );

            float DivideOrZero(float a, float b)
            {
                if (b is 0f)
                    return 0f;

                return a / b;
            }
        }

        public static float[] ToArrayPositions(this Vector3 vector)
        {
            return new[] { vector.x, vector.y, vector.z };
        }
    }
}
