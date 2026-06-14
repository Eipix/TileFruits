using System;
using System.Collections.Generic;
using System.Threading;
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
        private readonly HashSet<TileTrayItem> _despawningTiles = new();

        [SerializeField] private RectTransform _content;
        [SerializeField] private LayoutGroup _separatorsLayoutGroup;
        [SerializeField] private Image _separatorPrefab;
        [SerializeField, Min(0)] private int _separatorPoolCapacity = 4;
        
        [SerializeField, Min(0f), BoxGroup("ItemsShift")]
        private float _itemsShiftDuration = 0.2f;
        
        [SerializeField, BoxGroup("ItemsShift")]
        private Ease _shiftEase = Ease.OutQuad;
        
        [SerializeField, OnValueChanged(nameof(UpdateLayout)), BoxGroup("Layout")]
        private float _startXOffset = -300f;
        
        [SerializeField, OnValueChanged(nameof(UpdateLayout)), BoxGroup("Layout")]
        private float _spacing = 5f;
        
        [SerializeField, OnValueChanged(nameof(UpdateLayout)), BoxGroup("Layout")]
        private float _width = 130f;
        
        public event Action<TileTrayItem> Added;
        
        private TileTrayItem.Pool _tilePool;
        private TileTraySettings _settings;
        
        private ComponentPool<Image> _separatorPool;
        private RectTransform _separatorsPoolParent;

        private CancellationTokenSource _waitForDespawnSource = new();
        
        private RectTransform SeparatorsContent => (RectTransform)_separatorsLayoutGroup.transform;

        private void UpdateLayout() => UpdateLayout(false);
        
        [Inject]
        private void Construct(TileTrayItem.Pool itemsPool, TileTraySettings settings)
        {
            _tilePool = itemsPool;
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

        public void Insert(TileConfig config, int modelIndex, Vector2 lastClickedTilePosition)
        {
            var item = _tilePool.Spawn(config, _content);
            int viewIndex = GetAdjustedIndex(modelIndex, config);
            
            _tiles.Insert(viewIndex, item);
            
            var tileRect = item.RectTransform;
            float startX = GetTileTargetX(viewIndex);
            tileRect.anchoredPosition = new Vector2(startX, 0); 
            
            UpdateLayout(true);
            Added?.Invoke(item);
            
            item.SetWorldPosition(lastClickedTilePosition);
            item.ReturnToTray();
        }
        
        private int GetAdjustedIndex(int modelIndex, TileConfig newConfig)
        {
            int validCount = 0;
            TileConfig lastValidConfig = null;
        
            for (int i = 0; i < _tiles.Count; i++)
            {
                if (validCount == modelIndex)
                {
                    if (lastValidConfig != newConfig)
                    {
                        while (i < _tiles.Count && _despawningTiles.Contains(_tiles[i]))
                            i++;
                    }
            
                    return i;
                }
                
                if (_despawningTiles.Contains(_tiles[i]) is false)
                {
                    lastValidConfig = _tiles[i].Config;
                    validCount++;
                }
            }
        
            return _tiles.Count;
        }

        public void Match(TileConfig config)
        {
            List<TileTrayItem> tilesToDespawn = new(MahjongConstants.TilesPerMatch);
            
            foreach (var tile in _tiles)
            {
                if (tile.Config == config && !_despawningTiles.Contains(tile))
                {
                    tilesToDespawn.Add(tile);
                    _despawningTiles.Add(tile);
                }
            }
            
            WaitForDespawn(tilesToDespawn).Forget();
        }

        private async UniTask WaitForDespawn(List<TileTrayItem> tilesToDespawn)
        {
            List<UniTask> returnTasks = new();
            
            foreach (var tile in tilesToDespawn)
            {
                if (tile.ReturningToTray.IsActive())
                    returnTasks.Add(tile.ReturningToTray.ToUniTask());
            }
            
            bool isCanceled = await UniTask.WhenAll(returnTasks)
                .AttachExternalCancellation(_waitForDespawnSource.Token)
                .SuppressCancellationThrow();
            
            if (isCanceled)
                return;

            List<UniTask> hideTasks = new();
            
            foreach (var tile in tilesToDespawn)
                hideTasks.Add(tile.Hide().ToUniTask());
            
            bool ishidingCanceled = await UniTask.WhenAll(hideTasks)
                .AttachExternalCancellation(_waitForDespawnSource.Token)
                .SuppressCancellationThrow();;

            if (ishidingCanceled)
                return;
            
            foreach (var tile in tilesToDespawn)
            {
                _tiles.Remove(tile);
                _despawningTiles.Remove(tile);
                _tilePool.Despawn(tile);
            }
            
            UpdateLayout(true);
        }

        public void Clear()
        {
            _waitForDespawnSource.Cancel();
            _waitForDespawnSource.Dispose();
            _waitForDespawnSource = new();
            
            foreach (var item in _tiles)
                _tilePool.Despawn(item);
            
            _tiles.Clear();
            _despawningTiles.Clear();
        }

        private void UpdateLayout(bool animate)
        {
            for (int i = 0; i < _tiles.Count; i++)
            {
                var tile = _tiles[i];
                var tileRect = tile.RectTransform;
                
                float targetX = GetTileTargetX(i);
                
                tile.Shifting?.Kill();
                
                if (animate)
                {
                    tile.ShiftTo(targetX, _itemsShiftDuration, _shiftEase);
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
        
        private void OnDestroy()
        {
            if (_waitForDespawnSource != null)
            {
                _waitForDespawnSource.Cancel();
                _waitForDespawnSource.Dispose();
            }
        }
    }
}
