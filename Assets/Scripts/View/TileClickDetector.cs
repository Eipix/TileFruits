using System;
using System.Collections.Generic;
using _Commons.Scripts.UI;
using Commons.Systems;
using Commons.Systems.PauseManager;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using Zenject;

using PlayerInput = Input.PlayerInput;

namespace Gameplay
{
    public class TileClickDetector : IInitializable, IDisposable, IPausable
    {
        public event Action<Tile> TileClicked;
        
        private InputAction _playerClick;
        private InputAction _uiPoint;
        private IRegistry<IPausable> _pausableRegistry;
        private UIManager _uiManager;
        private Camera _mainCamera;

        public bool Enabled { get; private set; } = true;

        [Inject]
        private void Construct(PlayerInput playerInput,
            IRegistry<IPausable> pausableRegistry,
            UIManager uiManager)
        {
            _playerClick = playerInput.Player.Click;
            _uiPoint = playerInput.UI.Point;
            
            _pausableRegistry = pausableRegistry;
            _uiManager = uiManager;
            _mainCamera = Camera.main;
        }
        
        public void Initialize()
        {
            _uiManager.InputBlockRequired += OnInputBlockRequired;
            _pausableRegistry.Register(this);
            SubscribeToClick();
        }

        public void Dispose()
        {
            _uiManager.InputBlockRequired -= OnInputBlockRequired;
            _pausableRegistry.Unregister(this);
            
            if(_playerClick == null || _uiPoint == null)
                return;
            
            UnsubscribeToClick();
            _playerClick = null;
            _uiPoint = null;
        }

        private void OnInputBlockRequired(bool block) => Enabled = block is false;

        private void SubscribeToClick() =>  _playerClick.performed += OnClick;
        private void UnsubscribeToClick() =>  _playerClick.performed -= OnClick;

        private void OnClick(InputAction.CallbackContext ctx)
        {
            if (Enabled is false)
                return;
            
            var clickPosition = _uiPoint.ReadValue<Vector2>();
            
            if(IsPointerOverUI(clickPosition))
                return;

            Ray ray = _mainCamera.ScreenPointToRay(clickPosition);
            var hits = Physics2D.RaycastAll(ray.origin, ray.direction);
            
            if (hits.Length == 0)
                return;

            if (TryGetHighestLayer(hits, out Tile tile))
                TileClicked?.Invoke(tile);
        }
        
        private bool IsPointerOverUI(Vector2 point)
        {
            if (EventSystem.current == null)
                return false;

            PointerEventData eventData = new PointerEventData(EventSystem.current);
            
            eventData.position = point;

            List<RaycastResult> results = new();
            EventSystem.current.RaycastAll(eventData, results);

            return results.Count > 0;
        }

        private bool TryGetHighestLayer(RaycastHit2D[] hits, out Tile topTile)
        {
            topTile = null;
            float maxZ = float.MinValue;

            foreach (var hit in hits)
            {
                if (hit.collider.TryGetComponent<Tile>(out var tile) is false)
                    continue;
                
                float layer = tile.SortingOrder;
                
                if (layer > maxZ)
                {
                    maxZ = layer;
                    topTile = tile;
                }
            }
            
            return topTile != null;
        }

        public void Pause() => UnsubscribeToClick();

        public void Resume() => SubscribeToClick();
    }
}
