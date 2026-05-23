using System;
using System.Collections.Generic;
using Commons.Extensions;
using Commons.Pools;
using Commons.Systems.PauseManager;
using Commons.Systems.Save;
using JetBrains.Annotations;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.Audio;
using Zenject;

namespace Commons.Systems.AudioManager
{
    public class AudioManager : MonoBehaviour, IPausable
    {
        private readonly Dictionary<AudioResource, float> _lastPlayedTimes = new();
        private readonly List<Sound> _sounds = new();

        [SerializeField, Min(0f)] private float _minThrottleCooldown = 0.05f;
        [SerializeField, Min(0f)] private int _defaultPoolSize = 10;
        
        [SerializeField] private AudioMixerGroup _soundDefaultMixer;
        [SerializeField] private AudioMixerGroup _unscaledSoundMixer;
        [SerializeField] private AudioSource _soundPrefab;
        [SerializeField] private AudioSource _music;

        private ISaveSystem _saveSystem;
        private ComponentPool<AudioSource> _soundsPool;
        private IRegistry<IPausable> _pauseRegistry;

        public float MusicVolume { get; private set; }
        public float SoundVolume { get; private set; }
        public bool IsPaused { get; private set; }

        [Inject]
        private void Construct(ISaveSystem saveSystem, IRegistry<IPausable> pauseRegistry)
        {
            _saveSystem = saveSystem;
            _pauseRegistry = pauseRegistry;
            _pauseRegistry.Register(this);

            MusicVolume = saveSystem.Load(SaveKeys.MusicVolumeFloat, 0.5f);
            SoundVolume = saveSystem.Load(SaveKeys.SoundVolumeFloat, 0.5f);

            _music.volume = MusicVolume;
            _soundsPool = new(_soundPrefab, transform, defaultCapacity: _defaultPoolSize);
            _soundsPool.Prewarm();
        }

        private void OnDestroy()
        {
            _pauseRegistry?.Unregister(this);
            Stop();
        }

        private void Update()
        {
            if (IsPaused)
                return;

            ForEachSound((sound, index) =>
            {
                if (sound is { DestroyOnFinish: true, IsFinished: true })
                {
                    ReleaseSound(sound, index);
                }
            });
        }

        public void PlayMusic(AudioResource audioResource, bool loop = true)
        {
            _music.loop = loop;
            _music.Play(audioResource);
        }

        public bool HasMusic(AudioResource music)
            => music != null && _music.resource == music;

        public AudioSource GetNewSound(Transform parent = null, bool isUnscaled = false)
        {
            var audioSource = GetAudioSource(parent, false);

            audioSource.outputAudioMixerGroup = isUnscaled
                ? _unscaledSoundMixer
                : _soundDefaultMixer;

            return audioSource;
        }

        public void PlayOneShot(AudioResource audioResource, Transform parent = null,
            bool randomizePitch = true, float throttleCooldown = 0.05f,
            int priority = 128, bool isUnscaled = false, float delay = 0f)
        {
            SoundSettings settings = new(audioResource, isUnscaled
                ? _unscaledSoundMixer
                : _soundDefaultMixer, priority, throttleCooldown, randomizePitch, delay);

            PlayOneShot(settings, parent);
        }

        public void PlayOneShot(SoundSettings settings, Transform parent = null)
        {
            float throttleCooldown = settings.ThrottleCooldown;
            var audioResource = settings.Resource;
            var mixer = settings.Mixer;

            if(throttleCooldown > 0f)
            {
                if (throttleCooldown < _minThrottleCooldown)
                    throttleCooldown = _minThrottleCooldown;

                if (_lastPlayedTimes.TryGetValue(audioResource, out float lastPlayedTime))
                {
                    if (Time.unscaledTime - lastPlayedTime < throttleCooldown)
                        return;
                }
            }

            _lastPlayedTimes[audioResource] = Time.unscaledTime;

            var audioSource = GetAudioSource(parent ?? transform, true);
            audioSource.Configure(settings);

            if (mixer == null)
                audioSource.outputAudioMixerGroup = _soundDefaultMixer;

            if (IsPaused is false || mixer.audioMixer.updateMode is AudioMixerUpdateMode.UnscaledTime)
                audioSource.PlayDelayed(settings.Delay);
        }

        private void RegisterSound(Sound sound)
        {
            sound.SetVolume(SoundVolume);
            _sounds.Add(sound);
        }

        public void UnregisterSound(params AudioSource[] audioSources)
        {
            foreach (var audioSource in audioSources)
                UnregisterSound(audioSource);
        }

        public void UnregisterSound(AudioSource audioSource)
        {
            if (audioSource == null)
            {
                Debug.LogError("Failed to unregister AudioSource: it is null.");
                return;
            }

            for (int i = _sounds.Count - 1; i >= 0; i--)
            {
                var sound = _sounds[i];

                if (sound.IsDisposed)
                {
                    ReleaseSound(sound, i);
                    continue;
                }

                if (sound.Source.Equals(audioSource))
                {
                    ReleaseSound(sound, i);
                    return;
                }
            }

            Debug.LogError($"Failed to destroy AudioSource '{audioSource.name}'." +
                           "The AudioSource was not created or managed by the AudioManager. " +
                           "Ensure that all AudioSources are instantiated through the AudioManager.");
        }

        public void Stop()
        {
            StopMusic();
            StopSounds();
        }

        public bool TryStopMusic(AudioResource music)
        {
            if(HasMusic(music))
            {
                StopMusic();
                return true;
            }
            return false;
        }

        public void StopMusic() => _music.Stop();

        public void StopSounds() => ForEachSound((sound, _) => sound.Stop());

        [Button]
        public void Pause()
        {
            PauseSounds();
            IsPaused = true;
        }

        [Button]
        public void Resume()
        {
            IsPaused = false;
            ResumeSounds();
        }

        public void MuteMusic() => _music.mute = true;
        public void UnmuteMusic() => _music.mute = false;

        public void MuteSounds() => ForEachSound((sound, _) => sound.Mute = true);
        public void UnmuteSounds() => ForEachSound((sound, _) => sound.Mute = false);

        public void PauseMusic() => _music.Pause();
        public void ResumeMusic() => _music.UnPause();

        private void PauseSounds() => ForEachSound((sound, _) => sound.Pause());
        private void ResumeSounds() => ForEachSound((sound, _) => sound.Resume());

        public void SetMusicVolume(float volume)
        {
            volume = Mathf.Clamp01(volume);
            _saveSystem.Save(SaveKeys.MusicVolumeFloat, volume);
            
            MusicVolume = volume;
            _music.volume = MusicVolume;
        }

        public void SetSoundVolume(float volume)
        {
            volume = Mathf.Clamp01(volume);
            _saveSystem.Save(SaveKeys.SoundVolumeFloat, volume);
            
            SoundVolume = volume;
            ForEachSound((sound, _) => sound.SetVolume(SoundVolume));
        }

        private void ReleaseSound(Sound sound, int index)
        {
            _sounds.RemoveAt(index);
            var source = sound.Source;

            if (source != null)
            {
                source.UnPause();
                source.Stop();
                source.resource = null;
                source.clip = null;
                source.pitch = 1f;
                source.priority = 128;
                source.loop = false;
                source.outputAudioMixerGroup = _soundDefaultMixer;

                if (source.transform.IsChildOf(transform) is false)
                    sound.Source.transform.SetParent(transform, false);

                _soundsPool.Release(sound.Source);
            }

            sound.Dispose();
        }

        private AudioSource GetAudioSource([CanBeNull] Transform parent, bool destroyOnFinish)
        {
            var audioSource = _soundsPool.Get();
            audioSource.transform.SetParent(parent ?? transform, false);

            var sound = new Sound(audioSource, destroyOnFinish);
            RegisterSound(sound);

            return audioSource;
        }

        private void ForEachSound(Action<Sound, int> action)
        {
            for (int i = _sounds.Count - 1; i >= 0; i--)
            {
                var sound = _sounds[i];

                if (sound.IsDisposed)
                {
                    _sounds.RemoveAt(i);
                    continue;
                }

                action.Invoke(sound, i);
            }
        }
    }
}
