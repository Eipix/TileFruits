using Commons.ScriptableObjects;
using NaughtyAttributes;
using UnityEngine;

namespace Gameplay
{
    [CreateAssetMenu(menuName = "Tiles/Tile Config")]
    public class TileConfig : GUIDScriptableObject
    {
        [field: SerializeField, ShowAssetPreview]
        public Sprite Symbol { get; private set; }
    }
}
