using System;
using Commons.Extensions;
using DG.Tweening;
using Effects;
using JetBrains.Annotations;
using UI;
using UnityEngine;

namespace Commons
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(CanvasGroup))]
    public class Window : MonoBehaviour, IWindow
    {
        [SerializeField, CanBeNull] private UIAnimation _windowAnimation;

        public event Action Opening;
        public event Action Closing;

        private CanvasGroup _canvasGroup;

        public bool IsOpened { get; private set; }
        public bool IsClosed => IsOpened is false;

        private void Awake()
        {
            _canvasGroup = GetComponent<CanvasGroup>();
            _canvasGroup.Hide();
        }

        private void OnDisable() => Close(true);

        public void Open() => Open(false);

        public void Open(bool silent)
        {
            if (IsOpened)
                return;

            if (silent is false)
                Opening?.Invoke();

            _canvasGroup.Show();

            if (_windowAnimation != null)
                _windowAnimation.Show();

            IsOpened = true;
            OnOpen();
        }

        protected virtual void OnOpen() { }

        public void Close() => Close(false);

        public void Close(bool silent)
        {
            if (IsClosed)
                return;

            if (silent is false)
                Closing?.Invoke();

            if (_windowAnimation != null)
                _windowAnimation.Close().OnComplete(_canvasGroup.Hide);
            else
                _canvasGroup.Hide();

            IsOpened = false;
            OnClose();
        }

        protected virtual void OnClose() { }
    }
}
