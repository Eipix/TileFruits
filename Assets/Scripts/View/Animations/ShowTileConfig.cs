using System;
using DG.Tweening;
using UnityEngine;

namespace View.Animations
{
    [Serializable]
    public class ShowTileAnimationConfig
    {
        [field: SerializeField] public float Duration { get; private set; } = 0.3f;
        [field: SerializeField] public float StepDelay { get; private set; } = 0.01f;
        [field: SerializeField] public Ease Ease { get; private set; } = Ease.Linear;
        [field: SerializeField] public float OverShoot { get; private set; } = 1f;
    }
}
