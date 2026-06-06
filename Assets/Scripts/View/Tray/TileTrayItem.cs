using Gameplay;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace UI.Tray
{
    public class TileTrayItem : MonoBehaviour
    {
        [SerializeField] private RectTransform _iconsParent;
        [SerializeField] private Image _icon;
        
        public RectTransform IconsParent => _iconsParent;
        public RectTransform RectTransform { get; private set; }
        public TileConfig Config { get; private set; }
        
        public void Awake() => RectTransform = transform as RectTransform;
        
        public class Pool : MonoMemoryPool<TileConfig, RectTransform, TileTrayItem>
        {
            protected override void Reinitialize(TileConfig config, RectTransform parent, TileTrayItem item)
            {
                base.Reinitialize(config, parent, item);
                
                item.Config = config;
                item._icon.sprite = config.Symbol;
                item.RectTransform.SetParent(parent);
            }
        }
    }
}
