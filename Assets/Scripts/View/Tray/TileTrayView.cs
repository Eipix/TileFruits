using System;
using System.Collections.Generic;
using Commons.Extensions;
using Commons.Pools;
using Constants;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using NaughtyAttributes;
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

        [SerializeField] private RectTransform _content;
        [SerializeField] private LayoutGroup _separatorsLayoutGroup;
        [SerializeField] private Image _separatorPrefab;
        [SerializeField, Min(0)] private int _separatorPoolCapacity = 4;
        
        [SerializeField, OnValueChanged(nameof(UpdateLayout))]
        private float _startXOffset = -300f;
        
        [SerializeField, OnValueChanged(nameof(UpdateLayout))]
        private float _spacing = 5f;
        
        [SerializeField, OnValueChanged(nameof(UpdateLayout))]
        private float _width = 130f;
        
        public event Action<TileTrayItem> Added;
        
        private TileTrayItem.Pool _itemPool;
        private TileTraySettings _settings;
        
        private ComponentPool<Image> _separatorPool;
        private RectTransform _separatorsPoolParent;
        
        private RectTransform SeparatorsContent => (RectTransform)_separatorsLayoutGroup.transform;

        private void UpdateLayout() => UpdateLayout(false);
        
        [Inject]
        private void Construct(TileTrayItem.Pool itemsPool, TileTraySettings settings)
        {
            _itemPool = itemsPool;
            _settings = settings;
        }
        
        public void Initialize()
        {
            _separatorsPoolParent = InstantiateExtensions
                .Instantiate<RectTransform>(transform, "SeparatorsPool");
            
            _separatorsPoolParent.anchoredPosition3D = Vector3.zero;

            _separatorPool = new(_separatorPrefab,
                _separatorsPoolParent,
                actionOnGet: OnGet,
                defaultCapacity: _separatorPoolCapacity);
            
            _separatorPool.Prewarm();
            UpdateSeparatorsCount(_settings.Capacity);
            void OnGet(Image separator) => separator.rectTransform.SetParent(SeparatorsContent);
        }

        private void Start() => _separatorsLayoutGroup.RebuildAndDisable();

        public void Insert(TileConfig config, int index)
        {
            var item = _itemPool.Spawn(config, _content);
            _tiles.Insert(index, item);

            var tileRect = item.RectTransform;
            float startX = GetTileTargetX(index);
            tileRect.anchoredPosition = new Vector2(startX, 0); 
            
            UpdateLayout(true);
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
            
            UpdateLayout(true);
        }

        public void Clear()
        {
            foreach (var item in _tiles)
                _itemPool.Despawn(item);
            
            _tiles.Clear();
        }

        private void UpdateLayout(bool animate)
        {
            for (int i = 0; i < _tiles.Count; i++)
            {
                var tileRect = _tiles[i].RectTransform;
                float targetX = GetTileTargetX(i);

                if (animate)
                {
                    tileRect.DOAnchorPosX(targetX, 0.2f).SetEase(Ease.OutQuad);
                }
                else
                {
                    var position = tileRect.anchoredPosition;
                    position.x = targetX;
                    tileRect.anchoredPosition = position;
                }
            }
        }
        
        private float GetTileTargetX(int index)
        {
            return _startXOffset + (index * (_width + _spacing));
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
