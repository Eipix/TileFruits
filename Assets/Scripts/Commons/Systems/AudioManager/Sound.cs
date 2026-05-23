using System;
using UnityEngine;

namespace Commons.Systems.AudioManager
{
    public class Sound : IDisposable
    {
        private bool _isPaused;

        public bool Mute
        {
            get
            {
                if(Source == null)
                {
                    Debug.LogError("Sound: Source is null");
                    return false;
                }

                return Source.mute;
            }
            set
            {
                if (IsDisposed)
                    return;

                Source.mute = value;
            }
        }

        public AudioSource Source { get; private set; }
        public bool DestroyOnFinish { get; private set; }

        public bool IsDisposed => Source == null;
        public bool IsFinished => Source.isPlaying is false && _isPaused is false;

        public Sound(AudioSource source, bool destroyOnFinish)
        {
            DestroyOnFinish = destroyOnFinish;
            Source = source;
        }

        public void SetVolume(float volume)
        {
            if (IsDisposed)
                return;

            Source.volume = volume;
        }

        public void Pause()
        {
            if (IsDisposed)
                return;

            Source.Pause();
            _isPaused = true;
        }

        public void Resume()
        {
            if (IsDisposed)
                return;

            Source.UnPause();
            _isPaused = false;
        }

        public void Stop()
        {
            if (IsDisposed)
                return;

            Source.Stop();
        }

        public void Dispose()
        {
            if (IsDisposed)
                return;

            Source.Stop();
            Source = null;
        }
    }
}
