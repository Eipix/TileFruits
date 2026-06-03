using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace Commons.Extensions
{
    public static class DotweenExtensions
    {
        public static TweenerCore<float, float, FloatOptions> DOWeight(this Volume target, float endValue, float duration)
        {
            endValue = Mathf.Clamp(endValue, 0.0f, 1.0f);

            return DOTween.To(Getter, Setter, endValue, duration);

            float Getter() => target.weight;
            void Setter(float value) => target.weight = value;
        }

        public static Tween DOFade(this DecalProjector target, float endValue, float duration)
        {
            endValue = Mathf.Clamp(endValue, 0.0f, 1.0f);

            if (duration <= 0f)
                duration = 0f;

            return DOTween.To(Getter, Setter, endValue, duration);

            float Getter() => target.fadeFactor;
            void Setter(float value) => target.fadeFactor = value;
        }

        public static void CompleteIfActive(this Tween tween, bool withCallbacks = false)
        {
            if (tween.IsActive())
                tween.Complete(withCallbacks);
        }
    }
}
