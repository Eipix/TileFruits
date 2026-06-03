using Commons;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace UI
{
    public class VictoryWindow : Window
    {
        [SerializeField] private Button _closeButton;
        [SerializeField] private Button _nextLevelButton;

        private UnityAction _onNextLevel;
        
        public void Setup(UnityAction onNextLevel)
        {
            _onNextLevel = onNextLevel;
        }

        protected override void OnOpen()
        {
            _closeButton.onClick.AddListener(Close);
            _closeButton.onClick.AddListener(_onNextLevel);
            
            _nextLevelButton.onClick.AddListener(Close);
            _nextLevelButton.onClick.AddListener(_onNextLevel);
        }
        
        protected override void OnClose()
        {
            _closeButton.onClick.RemoveListener(Close);
            _closeButton.onClick.RemoveListener(_onNextLevel);
            
            _nextLevelButton.onClick.RemoveListener(Close);
            _nextLevelButton.onClick.RemoveListener(_onNextLevel);
        }
    }
}
