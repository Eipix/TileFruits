using System;
using _Commons.Scripts.UI;
using Commons.Systems;
using Commons.Systems.PauseManager;
using UnityEngine;
using UnityEngine.InputSystem;
using Zenject;

using PlayerInput = Input.PlayerInput;

namespace Gameplay
{
    public class TileClickDetector : IInitializable, IDisposable, IPausable
    {
        public event Action<Tile> TileClicked;
        
        private PlayerInput _playerInput;
        private IRegistry<IPausable> _pausableRegistry;
        private UIManager _uiManager;
        private Camera _mainCamera;

        public bool Enabled { get; private set; } = true;

        [Inject]
        private void Construct(PlayerInput playerInput,
            IRegistry<IPausable> pausableRegistry,
            UIManager uiManager)
        {
            _playerInput = playerInput;
            _pausableRegistry = pausableRegistry;
            _uiManager = uiManager;
        }
        
        public void Initialize()
        {
            _uiManager.InputBlockRequired += OnInputBlockRequired;
            _pausableRegistry.Register(this);
            SubscribeToClick();
            _mainCamera = Camera.main;
        }

        private void OnInputBlockRequired(bool block) => Enabled = block is false;

        private void SubscribeToClick() =>  _playerInput.UI.Click.performed += OnClick;
        private void UnsubscribeToClick() =>  _playerInput.UI.Click.performed -= OnClick;

        private void OnClick(InputAction.CallbackContext ctx)
        {
            if (Enabled is false)
                return;
            
            var clickPosition = _playerInput.UI.Point.ReadValue<Vector2>();

            Ray ray = _mainCamera.ScreenPointToRay(clickPosition);
            var hits = Physics2D.RaycastAll(ray.origin, ray.direction);
            
            if (hits.Length == 0)
                return;

            if (TryGetHighest(hits, out Tile tile))
                TileClicked?.Invoke(tile);
        }

        private bool TryGetHighest(RaycastHit2D[] hits, out Tile topTile)
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

        public void Dispose()
        {
            _uiManager.InputBlockRequired -= OnInputBlockRequired;
            _pausableRegistry.Unregister(this);
            
            if(_playerInput == null)
                return;
            
            UnsubscribeToClick();
            _playerInput = null;
        }

        public void Pause() => UnsubscribeToClick();

        public void Resume() => SubscribeToClick();
    }
}
