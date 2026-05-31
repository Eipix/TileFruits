using Generator;
using UnityEngine;

namespace Gameplay.Levels
{
    [CreateAssetMenu(menuName = "Levels/Level")]
    public class Level : ScriptableObject
    {
        [field: SerializeField] public GeneratorConfig GeneratorConfig { get; private set; }
    }
}
