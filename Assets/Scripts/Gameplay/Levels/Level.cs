using Commons.ScriptableObjects;
using Generator;
using UnityEngine;

namespace Gameplay.Levels
{
    [CreateAssetMenu(menuName = "Levels/Level")]
    public class Level : GUIDScriptableObject
    {
        [field: SerializeField] public GeneratorConfig GeneratorConfig { get; private set; }
    }
}
