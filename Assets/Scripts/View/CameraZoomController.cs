using EmeraldPowder.CameraScaler;
using Generator;
using NaughtyAttributes;
using UnityEngine;
using Zenject;

namespace View
{
    [RequireComponent(typeof(CameraScaler))]
    public class CameraZoomController : MonoBehaviour
    {
        [SerializeField] private Vector2 _relativePadding = new(1.05f, 1.15f);
        [SerializeField] private Vector2 _absolutePadding = new(2f, 5f);
        
        private MapVisualizer _mapVisualizer;
        private CameraScaler _cameraScaler;
        private Camera _mainCamera;

        [ShowNativeProperty] public float Zoom => _cameraScaler.CameraZoom;

        [Inject]
        private void Construct(
            MapVisualizer mapVisualizer)
        {
            _mapVisualizer = mapVisualizer;
            _cameraScaler = GetComponent<CameraScaler>();
            _mainCamera = Camera.main;
        }

        private void LateUpdate() => AdaptZoom();

        private void AdaptZoom()
        {
            Vector2 size = _mapVisualizer.Size;

            if (size == Vector2.zero)
                return;

            size *= _relativePadding + _absolutePadding;

            float aspect = _mainCamera.aspect;
            float requiredSizeByHeight = size.y / 2f;
            float requiredSizeByWidth = (size.x / 2f) / aspect;
        
            float targetOrthoSize = Mathf.Max(requiredSizeByHeight, requiredSizeByWidth);

            float currentOrthoSize = _mainCamera.orthographicSize;
            float currentZoom = _cameraScaler.CameraZoom;

            if (Mathf.Approximately(currentZoom, 0f)) 
                currentZoom = 1f;

            float baseOrthoSize = currentOrthoSize * currentZoom;

            float requiredZoom = baseOrthoSize / targetOrthoSize;
            _cameraScaler.CameraZoom = requiredZoom;
        }
    }
}
