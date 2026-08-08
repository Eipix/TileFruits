using System.Linq;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

namespace Commons.Extensions
{
    public static class RendererExtensions
    {
        /// <param name="alpha">Value from 0 to 1</param>
        public static void SetAlpha(this SpriteRenderer spriteRenderer, float alpha)
        {
            alpha = Mathf.Clamp(alpha, 0f, 1f);
            
            var color = spriteRenderer.color;
            color.a = alpha;
            spriteRenderer.color = color;
        }

        /// <param name="alpha">Value from 0 to 1</param>
        public static void SetAlpha(this MaskableGraphic maskableGraphic, float alpha)
        {
            alpha = Mathf.Clamp(alpha, 0f, 1f);
            
            var color = maskableGraphic.color;
            color.a = alpha;
            maskableGraphic.color = color;
        }
    }
}
