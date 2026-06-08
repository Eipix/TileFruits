using UnityEngine;
using UnityEngine.UI;

namespace Commons.Extensions
{
    public static class LayoutGroupExtensions
    {
        public static void RebuildAndDisable(this LayoutGroup layoutGroup)
        {
            layoutGroup.enabled = true;
            
            var rect = (RectTransform)layoutGroup.transform;
            LayoutRebuilder.ForceRebuildLayoutImmediate(rect);
            
            layoutGroup.enabled = false;
        }
    }
}
