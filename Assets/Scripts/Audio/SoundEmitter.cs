using Commons.Extensions;
using Commons.Systems.AudioManager;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.Audio;
using Zenject;

namespace Commons.Sounds
{
    public class SoundEmitter : MonoBehaviour
    {
        [SerializeField] private bool _loop;
        [SerializeField, Min(0f)] private float _delay;

        private AudioManager _audioManager;
        [CanBeNull] private AudioSource _audioSource;

        [Inject]
        private void Construct(AudioManager audioManager)
        {
            _audioManager = audioManager;
        }

        private void OnDisable()
        {
            if (_audioSource != null)
                _audioManager.UnregisterSound(_audioSource);
        }

        public void PlayOneShot(SoundSettings settings)
            => _audioManager.PlayOneShot(settings);

        public void PlayOneShot(AudioResource resource)
            => _audioManager.PlayOneShot(resource, delay: _delay);

        public void Play(AudioResource resource)
        {
            if (_audioSource == null)
            {
                _audioSource = _audioManager.GetNewSound();
                _audioSource!.loop = _loop;
            }

            _audioSource.Play(resource, _delay);
        }

        public void Stop()
        {
            if (_audioSource != null)
            {
                _audioManager.UnregisterSound(_audioSource);
                _audioSource = null;
            }
        }
    }
}
