using Gameplay;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace View.Windows.Collection
{
    public class CollectionItem : MonoBehaviour
    {
        [SerializeField] private RectTransform _tile;
        [SerializeField] private Image _tileIcon;
        [SerializeField] private Image _lock;

        private bool _isUnlocked;
        
        public string Id { get; private set; }
        
        public bool IsUnlocked
        {
            get => _isUnlocked;
            set
            {
                _isUnlocked = value;
                
                if (_isUnlocked)
                {
                    _lock.gameObject.SetActive(false);
                    _tile.gameObject.SetActive(true);
                }
                else
                {
                    _lock.gameObject.SetActive(true);
                    _tile.gameObject.SetActive(false);
                }
            }
        }

        [Inject]
        private void Construct(TileConfig tileConfig, RectTransform parent)
        {
            _tileIcon.sprite = tileConfig.Symbol;
            Id = tileConfig.Id;
            transform.SetParent(parent, false);
            IsUnlocked = false;
        }

        public class Factory : PlaceholderFactory<TileConfig, RectTransform, CollectionItem> { }
    }
}
