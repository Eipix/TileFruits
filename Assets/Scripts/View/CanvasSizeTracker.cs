using System;
using UnityEngine;
using UnityEngine.UI;

namespace View
{
    [RequireComponent(typeof(CanvasScaler))]
    public class CanvasSizeTracker : MonoBehaviour
    {
        public event Action Changed;

        public RectTransform RectTransform;
        
        public Vector2 ReferenceResolution;

        private void Awake()
        {
            ReferenceResolution = GetComponent<CanvasScaler>().referenceResolution;
            RectTransform = (RectTransform)transform;
        }

        private void OnRectTransformDimensionsChange()
        {
            Changed?.Invoke();
        }
    }
}
