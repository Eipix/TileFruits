using System;
using DG.Tweening;
using UnityEngine;

namespace View.Animations
{
    [Serializable]
    public class HideAnimationConfig
    {
        [field: SerializeField, Min(0f)] public float Duration { get; private set; } = 0.3f;
        [field: SerializeField] public Ease Ease { get; private set; } = Ease.Linear;
    }
}
