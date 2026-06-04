using System;
using DG.Tweening;
using UnityEngine;

namespace _Commons.Scripts.Effects.Shakers
{
    [Serializable]
    public class ShakerConfig
    {
        [field: SerializeField] public float Duration { get; private set; } = 1f;
        [field: SerializeField] public Vector3 Strength  { get; private set; } = new(1f, 1f, 3f);
        [field: SerializeField] public int Vibrato  { get; private set; } = 20;
        [field: SerializeField] public float Randomness  { get; private set; } = 40f;
        [field: SerializeField] public bool Snapping  { get; private set; }
        [field: SerializeField] public bool FadeOut  { get; private set; } = true;
        [field: SerializeField] public ShakeRandomnessMode RandomnessMode  { get; private set; } = ShakeRandomnessMode.Full;
    }
}
