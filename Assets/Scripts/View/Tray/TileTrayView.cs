using System;
using System.Collections.Generic;
using Commons.Pools;
using Effects;
using UI.Tray;
using UnityEngine;
using UnityEngine.UI;
using Zenject;
using IInitializable = Zenject.IInitializable;

namespace Gameplay.Tray
{
    public class TileTrayView : MonoBehaviour, IInitializable
    {
        private readonly List<TileTrayItem> _items = new();
        private readonly List<Image> _separators = new();

        [SerializeField] private UIAnimation _insertAnimation;
        [SerializeField] private RectTransform _content;
        [SerializeField] private Image _separatorPrefab;
        [SerializeField, Min(0)] private int _separatorPoolCapacity = 4;

        public event Action<TileTrayItem> Added;
        
        [Inject] private TileTrayItem.Pool _itemPool;
        
        private ComponentPool<Image> _separatorPool;
        private RectTransform _separatorsPoolParent;

        public void Initialize()
        {
            var go = new GameObject();
            _separatorsPoolParent = go.AddComponent<RectTransform>();
            _separatorsPoolParent.SetParent(transform);
            _separatorsPoolParent.localScale = Vector3.one;
            _separatorsPoolParent.name = "SeparatorsPool";

            _separatorPool = new(_separatorPrefab,
                _separatorsPoolParent,
                actionOnGet: OnGet,
                defaultCapacity: _separatorPoolCapacity);
            
            _separatorPool.Prewarm();
            
            void OnGet(Image separator) => separator.rectTransform.SetParent(_content);
        }

        public void Insert(TileConfig config, int index)
        {
            var item = _itemPool.Spawn(config, _content);
            _items.Insert(index, item);
            
            UpdateSeparatorsCount();
            Reorder();
            
            Added?.Invoke(item);
        }

        public void Match(TileConfig config)
        {
            for (int i = _items.Count - 1; i >= 0; i--)
            {
                var item = _items[i];

                if (item.Config == config)
                {
                    _items.RemoveAt(i);
                    _itemPool.Despawn(item);
                }
            }

            UpdateSeparatorsCount();
            Reorder();
        }

        public void Clear()
        {
            foreach (var separator in _separators)
                _separatorPool.Release(separator);
            
            _separators.Clear();
            
            foreach (var item in _items)
                _itemPool.Despawn(item);
            
            _items.Clear();
        }
        
        private void Reorder()
        {
            for (int i = 0; i < _items.Count; i++)
                _items[i].transform.SetSiblingIndex(2 * i);
            
            for (int i = 0; i < _separators.Count; i++)
                _separators[i].transform.SetSiblingIndex(2 * i + 1);
        }

        private void UpdateSeparatorsCount()
        {
            int requiredCount = _items.Count >= 2 ? _items.Count - 1 : 0;
    
            while (_separators.Count != requiredCount)
            {
                if (requiredCount > _separators.Count)
                {
                    var separator = _separatorPool.Get();
                    _separators.Add(separator);
                }
                else if (requiredCount < _separators.Count)
                {
                    int lastIndex = _separators.Count - 1;
                    var last = _separators[lastIndex];
                    _separators.RemoveAt(lastIndex);
                    _separatorPool.Release(last);
                }
            }
        }
    }
}
