using UnityEngine;

namespace Gameplay
{
    public class TileMock : MonoBehaviour
    {
        [SerializeField] private SpriteRenderer _bone;
        
        public Vector3Int GridPosition { get; set; }

        public Color Color
        {
            get => _bone.color;
            set => _bone.color = value;
        }

        public int SortingOrder
        {
            get => _bone.sortingOrder;
            set => _bone.sortingOrder = value;
        }

        public Vector3Int GetGridSideByWorldPosition(Vector2 position)
        {
            Vector2 tileCenter = transform.position;

            Vector2 localHit = position - tileCenter;

            var size = _bone.bounds.size;
            
            float normX = localHit.x / (size.x * 0.5f);
            float normY = localHit.y / (size.y * 0.5f);

            const float centerThreshold = 0.33f; 

            int offsetX = 0;
            
            if (normX < -centerThreshold)
                offsetX = -1;
            
            else if (normX > centerThreshold)
                offsetX = 1;

            int offsetY = 0;
            
            if (normY < -centerThreshold)
                offsetY = -1;
            else if (normY > centerThreshold)
                offsetY = 1;

            return GridPosition + new Vector3Int(offsetX, offsetY);
        }
    }
}
