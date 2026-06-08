using System;
using System.Collections.Generic;
using Commons.Extensions;
using Commons.Pools;
using Constants;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UI.Tray;
using UnityEngine;
using UnityEngine.UI;
using Zenject;
using IInitializable = Zenject.IInitializable;

namespace Gameplay.Tray
{
    public class TileTrayView : MonoBehaviour, IInitializable
    {
        private readonly List<TileTrayItem> _tiles = new();
        private readonly List<Image> _separators = new();

        [SerializeField] private LayoutGroup _layoutGroup;
        [SerializeField] private LayoutGroup _separatorsLayoutGroup;
        [SerializeField] private Image _separatorPrefab;
        [SerializeField, Min(0)] private int _separatorPoolCapacity = 4;
        
        public event Action<TileTrayItem> Added;
        
        private TileTrayItem.Pool _itemPool;
        private TileTraySettings _settings;
        
        private ComponentPool<Image> _separatorPool;
        private RectTransform _separatorsPoolParent;
        
        public IReadOnlyList<TileTrayItem> Tiles => _tiles;
        private RectTransform Content => (RectTransform)_layoutGroup.transform;
        private RectTransform SeparatorsContent => (RectTransform)_separatorsLayoutGroup.transform;

        [Inject]
        private void Construct(TileTrayItem.Pool itemsPool, TileTraySettings settings)
        {
            _itemPool = itemsPool;
            _settings = settings;
        }
        
        public void Initialize()
        {
            _layoutGroup.enabled = false;

            _separatorsPoolParent = InstantiateExtensions
                .Instantiate<RectTransform>(transform, "SeparatorsPool");

            _separatorPool = new(_separatorPrefab,
                _separatorsPoolParent,
                actionOnGet: OnGet,
                defaultCapacity: _separatorPoolCapacity);
            
            _separatorPool.Prewarm();
            UpdateSeparatorsCount(_settings.Capacity);
            _separatorsLayoutGroup.RebuildAndDisable();
            void OnGet(Image separator) => separator.rectTransform.SetParent(SeparatorsContent);
        }

        public void Insert(TileConfig config, int index)
        {
            var item = _itemPool.Spawn(config, Content);
            _tiles.Insert(index, item);
            
            ReorderAndRebuild();
            
            Added?.Invoke(item);
        }

        public void Match(TileConfig config)
        {
            List<TileTrayItem> tilesToDespawn = new(MahjongConstants.TilesPerMatch);
            
            foreach (var tile in _tiles)
            {
                if (tile.Config == config)
                    tilesToDespawn.Add(tile);
            }
            
            WaitToDespawn(tilesToDespawn).Forget();
        }

        private async UniTask WaitToDespawn(List<TileTrayItem> tilesToDespawn)
        {
            foreach (var tile in tilesToDespawn)
            {
                var returning = tile.ReturningToTray;

                if (returning.IsActive())
                    await returning.AsyncWaitForCompletion().AsUniTask();
            }

            foreach (var tile in tilesToDespawn)
                tile.Hide().OnComplete(() =>
                {
                    _tiles.Remove(tile);
                    _itemPool.Despawn(tile);
                });
            
            foreach (var tile in tilesToDespawn)
            {
                var hiding = tile.Hiding;

                if (hiding.IsActive())
                    await hiding.AsyncWaitForCompletion().AsUniTask();
            }
            
            ReorderAndRebuild();
        }

        public void Clear()
        {
            foreach (var separator in _separators)
                _separatorPool.Release(separator);
            
            _separators.Clear();
            
            foreach (var item in _tiles)
                _itemPool.Despawn(item);
            
            _tiles.Clear();
        }
        
        private void ReorderAndRebuild()
        {
            for (int i = 0; i < _tiles.Count; i++)
                _tiles[i].transform.SetSiblingIndex(i);
            
            _layoutGroup.RebuildAndDisable();
        }

        private void UpdateSeparatorsCount(int count)
        {
            int requiredCount = count >= 2 ? count - 1 : 0;
    
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
