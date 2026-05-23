using UnityEngine;

namespace Commons.Extensions
{
    public static class LayerExtensions
    {
        public static bool IsInLayerMask(this GameObject obj, LayerMask mask)
        {
            return (mask.value & (1 << obj.layer)) != 0;
        }
    }
}
