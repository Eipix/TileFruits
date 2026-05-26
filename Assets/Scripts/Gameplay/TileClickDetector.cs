using System;
using Gameplay;
using UnityEngine;
using UnityEngine.InputSystem;
using Zenject;

namespace Input
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
            Debug.Log($"Click position: {clickPosition}");

            Ray ray = _mainCamera.ScreenPointToRay(clickPosition);

            var hit = Physics2D.Raycast(ray.origin, ray.direction);

            var hitObject = hit.collider;
            
            if (hitObject == null)
                return;

            Debug.Log($"Raycast hit object: {hitObject.name} in position {hit.point}");

            if (hitObject.TryGetComponent<Tile>(out var tile))
                TileClicked?.Invoke(tile);
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
