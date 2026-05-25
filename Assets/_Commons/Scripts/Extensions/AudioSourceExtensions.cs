using Commons.Systems.AudioManager;
using UnityEngine;
using UnityEngine.Audio;

namespace Commons.Extensions
{
    public static class AudioSourceExtensions
    {
        public static void Play(this AudioSource audioSource, AudioResource audioResource)
        {
            audioSource.resource = audioResource;
            audioSource.Play();
        }

        public static void Play(this AudioSource audioSource, AudioResource audioResource, float delay)
        {
            if(delay < 0)
                delay = 0;

            audioSource.resource = audioResource;
            audioSource.PlayDelayed(delay);
        }

        public static void Configure(this AudioSource audioSource, SoundSettings settings)
        {
            audioSource.resource = settings.Resource;
            audioSource.outputAudioMixerGroup = settings.Mixer;
            audioSource.pitch = settings.RandomizePitch ? Random.Range(0.9f, 1.1f) : 1f;
            audioSource.priority = settings.Priority;
        }
    }
}
