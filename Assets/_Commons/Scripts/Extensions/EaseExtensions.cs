using DG.Tweening;
using UnityEngine;

namespace Commons.Extensions
{
    public static class EaseExtensions
    {
        public static AnimationCurve ToAnimationCurve(this Ease ease)
        {
            AnimationCurve CurveIn(float sT, float eT) => new(new(0, 0, 0, sT), new(1, 1, eT, eT));
            AnimationCurve CurveOut(float sT, float eT) => new(new(0, 0, sT, sT), new(1, 1, 0, eT));
            AnimationCurve CurveInOut(float t) => new(new(0, 0, 0, 0), new(0.5f, 0.5f, t, t), new(1, 1, 0, 0));

             return ease switch
            {
                Ease.Linear => AnimationCurve.Linear(0, 0, 1, 1),

                Ease.InSine => CurveIn(0, 1.57f),
                Ease.OutSine => CurveOut(1.57f, 0),
                Ease.InOutSine => CurveInOut(1.57f),

                Ease.InQuad => CurveIn(0, 2f),
                Ease.OutQuad => CurveOut(2f, 0),
                Ease.InOutQuad => CurveInOut(2f),

                Ease.InCubic => CurveIn(0, 3f),
                Ease.OutCubic => CurveOut(3f, 0),
                Ease.InOutCubic => CurveInOut(3f),

                Ease.InQuart => CurveIn(0, 4f),
                Ease.OutQuart => CurveOut(4f, 0),
                Ease.InOutQuart => CurveInOut(4f),

                Ease.InQuint => CurveIn(0, 5f),
                Ease.OutQuint => CurveOut(5f, 0),
                Ease.InOutQuint => CurveInOut(5f),

                Ease.InExpo => CurveIn(0.1f, 7f),
                Ease.OutExpo => CurveOut(7f, 0.1f),
                Ease.InOutExpo => CurveInOut(7f),

                Ease.InCirc => CurveIn(0.4f, 5f),
                Ease.OutCirc => CurveOut(5f, 0.4f),
                Ease.InOutCirc => CurveInOut(3f),

                Ease.InBack => new(new(0, 0, -1, -1), new(1, 1, 4, 4)),
                Ease.OutBack => new(new(0, 0, 4, 4), new(1, 1, -1, -1)),
                Ease.InOutBack => CurveInOut(4f),

                Ease.InElastic or Ease.OutElastic or Ease.InOutElastic =>
                    new(
                        new (0f, 0f, 0f, 0f),
                        new (0.2f, -0.04f, -0.6f, -0.6f),
                        new (0.4f, 0.12f, 1.8f, 1.8f),
                        new (0.6f, -0.3f, -2.5f, -2.5f),
                        new (0.75f, 1.25f, 4.0f, 4.0f),
                        new (0.9f, 0.95f, -1.2f, -1.2f),
                        new (1f, 1f, 0.5f, 0f)
                    ),

                Ease.InBounce or Ease.OutBounce or Ease.InOutBounce =>
                    new(new(0, 0), new(0.35f, 0.8f), new(0.65f, 1f), new(1, 1)),
                
                Ease.Flash or Ease.InFlash or Ease.OutFlash or Ease.InOutFlash =>
                    new(new(0, 0), new(0.25f, 1), new(0.5f, 0), new(0.75f, 1), new(1, 0)),

                _ => AnimationCurve.Linear(0, 0, 1, 1)
            };
        }
    }
}
