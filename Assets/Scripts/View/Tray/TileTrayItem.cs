using Gameplay;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace UI.Tray
{
    public class TileTrayItem : MonoBehaviour
    {
        [SerializeField] private Image _icon;
        
        private RectTransform _rectTransform;
        
        public TileConfig Config { get; private set; }
        
        public void Awake() => _rectTransform = transform as RectTransform;
        
        public class Pool : MonoMemoryPool<TileConfig, RectTransform, TileTrayItem>
        {
            protected override void Reinitialize(TileConfig config, RectTransform parent, TileTrayItem item)
            {
                base.Reinitialize(config, parent, item);
                
                item.Config = config;
                item._icon.sprite = config.Symbol;
                item._rectTransform.SetParent(parent);
            }
        }
    }
}
