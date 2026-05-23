using System;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.Audio;

namespace Commons.Systems.AudioManager
{
    [Serializable]
    public struct SoundSettings
    {
        [field: SerializeField] public AudioResource Resource { get; private set; }
        [field: SerializeField, CanBeNull] public AudioMixerGroup Mixer { get; private set; }

        [field: SerializeField, Range(0f, 256f)] public int Priority { get; private set; }
        [field: SerializeField, Min(0f)] public float ThrottleCooldown { get; private set; }
        [field: SerializeField] public bool RandomizePitch { get; private set; }

        [field: SerializeField] public float Delay { get; private set; }

        public SoundSettings(AudioResource resource, AudioMixerGroup mixer,
            int priority, float throttleCooldown, bool randomizePitch, float delay = 0f)
        {
            Resource = resource;
            Mixer = mixer;
            Priority = priority;
            ThrottleCooldown = throttleCooldown;
            RandomizePitch = randomizePitch;
            Delay = delay;
        }
    }
}
