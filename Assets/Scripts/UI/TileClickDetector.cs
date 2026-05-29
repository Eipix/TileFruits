using System;
using UnityEngine;
using UnityEngine.InputSystem;
using Zenject;

using PlayerInput = Input.PlayerInput;

namespace Gameplay
{
    public class TileClickDetector : IInitializable, IDisposable
    {
        public event Action<Tile> TileClicked;
        
        [Inject] private PlayerInput _playerInput;
        private Camera _mainCamera;
        
        public void Initialize()
        {
            _playerInput.UI.Click.performed += OnClick;
            _mainCamera = Camera.main;
        }

        private void OnClick(InputAction.CallbackContext ctx)
        {
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
                
                float layer = tile.GridPosition.z;
                
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
            if(_playerInput == null)
                return;
            
            _playerInput.UI.Click.performed -= OnClick;
            _playerInput = null;
        }
    }
}
