using System.Linq;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

namespace Commons.Extensions
{
    public static class RendererExtensions
    {
        public static readonly int OpacityThreshold = Shader.PropertyToID("_OpacityThreshold");
        public static readonly int BaseColor = Shader.PropertyToID("_BaseColor");
        public static readonly int Surface = Shader.PropertyToID("_Surface");
        public static readonly int ZWrite = Shader.PropertyToID("_ZWrite");
        public static readonly int SrcBlend = Shader.PropertyToID("_SrcBlend");
        public static readonly int DstBlend = Shader.PropertyToID("_DstBlend");

        public static void SetAutodeskMaskedAlpha(this Renderer target, float alpha)
        {
            alpha = Mathf.Clamp(alpha, 0f, 1f);
            var material =  target.material;

            if (material.HasProperty(OpacityThreshold))
                material.SetFloat(OpacityThreshold, alpha);
            else
                Debug.LogError($"Has no property in {material.name} material on {target.gameObject.name} game object");
        }

        /// <summary>Only for lit shaders</summary>
        public static void SetLitAlpha(this Renderer renderer, float alpha)
        {
            var materials = renderer.materials;

            for (int i = 0; i < materials.Length; i++)
            {
                var material = materials[i];
                Color color = material.GetColor(BaseColor);

                if (alpha >= 0.99f)
                {
                    if (material.renderQueue is not (int)RenderQueue.Geometry)
                    {
                        SetupMaterialMode(material, isTransparent: false);
                    }

                    if (color.a < 1f)
                    {
                        color.a = 1f;
                        material.SetColor(BaseColor, color);
                    }
                }
                else
                {
                    if (material.renderQueue is not (int)RenderQueue.Transparent)
                    {
                        SetupMaterialMode(material, isTransparent: true);
                    }

                    if (Mathf.Abs(color.a - alpha) > 0.01f)
                    {
                        color.a = alpha;
                        material.SetColor(BaseColor, color);
                    }
                }
            }
        }

        private static void SetupMaterialMode(Material material, bool isTransparent)
        {
            if (isTransparent)
            {
                material.SetFloat(Surface, 1);
                material.SetFloat(ZWrite, 0);
                material.EnableKeyword("_SURFACE_TYPE_TRANSPARENT");

                material.SetInt(SrcBlend, (int)BlendMode.SrcAlpha);
                material.SetInt(DstBlend, (int)BlendMode.OneMinusSrcAlpha);

                material.renderQueue = (int)RenderQueue.Transparent;
            }
            else
            {
                material.SetFloat(Surface, 0);
                material.SetFloat(ZWrite, 1);
                material.DisableKeyword("_SURFACE_TYPE_TRANSPARENT");

                material.SetInt(SrcBlend, (int)BlendMode.One);
                material.SetInt(DstBlend, (int)BlendMode.Zero);

                material.renderQueue = (int)RenderQueue.Geometry;
            }
        }

        /// <param name="alpha">Value from 0 to 1</param>
        public static void Fade(this SpriteRenderer spriteRenderer, float alpha)
        {
            alpha = Mathf.Clamp(alpha, 0f, 1f);
            var oldColor = spriteRenderer.color;
            spriteRenderer.color = new Color(oldColor.r, oldColor.g, oldColor.b, alpha);
        }

        /// <param name="alpha">Value from 0 to 1</param>
        public static void Fade(this MaskableGraphic maskableGraphic, float alpha)
        {
            alpha = Mathf.Clamp(alpha, 0f, 1f);
            var oldColor = maskableGraphic.color;
            maskableGraphic.color = new Color(oldColor.r, oldColor.g, oldColor.b, alpha);
        }
    }
}
