using UnityEngine;

namespace Gameplay
{
    public class TileMock : MonoBehaviour
    {
        [SerializeField] private SpriteRenderer _bone;

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

        public Vector2 Size => _bone.sprite.bounds.size;
    }
}
