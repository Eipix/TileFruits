using System;
using System.Collections.Generic;
using UnityEngine;

namespace Commons.Systems.PauseManager
{
    public class PauseManager : IRegistry<IPausable>, IDisposable
    {
        private readonly List<IPausable> _pausables = new();

        public bool IsPaused { get; private set; }

        public void Register(IPausable pausable) => _pausables.Add(pausable);

        public void Unregister(IPausable pausable) => _pausables.Remove(pausable);

        public void Pause()
        {
            foreach (var pausable in _pausables)
                pausable.Pause();

            Time.timeScale = 0f;
            IsPaused = true;
        }

        public void Resume()
        {
            Time.timeScale = 1f;

            foreach (var pausable in _pausables)
                pausable.Resume();

            IsPaused = false;
        }


        public void Dispose()
        {
            Time.timeScale = 1f;
        }
    }
}
