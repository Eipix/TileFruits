using System;
using DG.Tweening;
using UnityEngine;

namespace View.Animations
{
    [Serializable]
    public class CollectAnimationConfig
    {
        [field: SerializeField, Min(0f)] public float MoveDuration { get; private set; } = 0.5f;
        [field: SerializeField] public Ease MoveEase { get; private set; } = Ease.Linear;
        
        [field: SerializeField] public Vector2 Punch { get; private set; } = Vector2.one;
        [field: SerializeField, Min(0f)] public float PunchDuration { get; private set; } = 0.3f;
        [field: SerializeField, Min(0f)] public int Vibrato { get; private set; } = 10;
        [field: SerializeField, Min(0f)] public float Elasticity { get; private set; } = 1f;
    }
}
