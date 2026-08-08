using System;
using System.Collections.Generic;
using Commons;
using Gameplay;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace View.Windows.Collection
{
    public class CollectionWindow : Window
    {
        private readonly List<CollectionItem> _items = new();
        
        [SerializeField] private Button _close;
        [SerializeField] private RectTransform _itemsParent;
        [SerializeField] private LayoutGroup _group;

        public event Action TilePointerDown;
        
        [Inject] private CollectionItem.Factory _itemsFactory;
        
        public void Add(TileConfig config)
        {
            var item = _itemsFactory.Create(config, _itemsParent);
            item.PointerDown += OnPointerDown;
            _items.Add(item);
        }

        private void OnPointerDown() => TilePointerDown?.Invoke();

        private void OnDestroy()
        {
            _items.ForEach(item => item.PointerDown -= OnPointerDown);
            _items.Clear();
        }

        public void ForceRebuildLayoutImmediate()
        {
            _group.enabled = true;
            LayoutRebuilder.ForceRebuildLayoutImmediate(_group.transform as RectTransform);
            _group.enabled = false;
        }

        public void Unlock(TileConfig config)
        {
            var targetItem = _items.Find(x => x.Id == config.Id);
            
            if (targetItem == null)
                throw new NullReferenceException($"Could not find tile {config.name}");
            
            targetItem.IsUnlocked = true;
        }

        protected override void OnOpen() => _close.onClick.AddListener(Close);

        protected override void OnClose() => _close.onClick.RemoveListener(Close);
    }
}
