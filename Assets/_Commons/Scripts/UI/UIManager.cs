using System;
using System.Collections.Generic;
using Commons;
using Commons.Extensions;
using UnityEngine;
using Zenject;

namespace _Commons.Scripts.UI
{
    public class UIManager : IInitializable, IDisposable
    {
        private readonly Dictionary<Type, Window> _windowsByType = new();

        public event Action<bool> InputBlockRequired;
        
        public int OpenedWindowsCount { get; private set; }

        public UIManager(List<Window> windows)
        {
            foreach (var window in windows)
                _windowsByType.Add(window.GetType(), window);
        }
        
        public void Initialize()
        {
            foreach (var window in _windowsByType.Values)
            {
                window.Opening += OnOpen;
                window.Closing += OnClose;
            }
        }

        private void OnClose()
        {
            OpenedWindowsCount = Math.Max(0, OpenedWindowsCount - 1);
            
            if(OpenedWindowsCount is 0)
                InputBlockRequired?.Invoke(false);
        }

        private void OnOpen()
        {
            OpenedWindowsCount++;
            
            if(OpenedWindowsCount is 1)
                InputBlockRequired?.Invoke(true);
        }

        public void Dispose()
        {
            foreach (var window in _windowsByType.Values)
            {
                window.Opening -= OnOpen;
                window.Closing -= OnClose;
            }
        }
        
        public T GetWindow<T>() where T : Window
        {
            if (_windowsByType.TryGetValue(typeof(T), out var window))
                return (T)window;
            
            return null;
        }

        public void OpenWindow<T>() where T : Window
        {
            var window = GetWindow<T>();
            window.Open();
        }

        public void CloseWindow<T>() where T : Window
        {
            var window = GetWindow<T>();
            window.Close();
        }

        public void CloseAllWindows() =>
            _windowsByType.Values.ForEach(window => window.Close());
    }
}
