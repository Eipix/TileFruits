using Gameplay.Tray;
using NaughtyAttributes;
using UnityEngine;
using Zenject;

namespace UI.Tray
{
    [RequireComponent(typeof(RectTransform))]
    public class TraySizeController : MonoBehaviour
    {
        [SerializeField] private float _tileWidth = 136f;
        [SerializeField] private float _separatorWidth = 11f;
        [SerializeField] private float _height = 100f;

        [Inject] private TileTraySettings _settings;
        
        private RectTransform _rectTransform;

        private void Awake() => _rectTransform = transform as RectTransform;

        private void Start() => SetSize();

        [Button]
        public void SetSize()
        {
            int capacity = _settings.Capacity;
            float tilesWidth = _tileWidth * capacity;
            float separatorsWidth = _separatorWidth * capacity - 1;
            
            Vector2 targetSize = new(tilesWidth + separatorsWidth, _height);
            _rectTransform.sizeDelta = targetSize;
        }
    }
}
