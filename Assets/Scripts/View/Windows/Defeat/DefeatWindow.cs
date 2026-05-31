using Commons;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace UI
{
    public class DefeatWindow : Window
    {
        [SerializeField] private Button _closeButton;
        [SerializeField] private Button _restartButton;

        private UnityAction _onRestart;
        
        public void Setup(UnityAction onRestart)
        {
            _onRestart = onRestart;
        }

        protected override void OnOpen()
        {
            _closeButton.onClick.AddListener(Close);
            _closeButton.onClick.AddListener(_onRestart);
            
            _restartButton.onClick.AddListener(Close);
            _restartButton.onClick.AddListener(_onRestart);
        }
        
        protected override void OnClose()
        {
            _closeButton.onClick.RemoveListener(Close);
            _closeButton.onClick.RemoveListener(_onRestart);
            
            _restartButton.onClick.RemoveListener(Close);
            _restartButton.onClick.RemoveListener(_onRestart);
        }
    }
}
