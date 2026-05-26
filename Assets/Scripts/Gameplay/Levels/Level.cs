using Generator;
using UnityEngine;

namespace Input.Levels
{
    [CreateAssetMenu(fileName = "Level")]
    public class Level : ScriptableObject
    {
        [field: SerializeField] public GeneratorConfig GeneratorConfig { get; private set; }
    }
}
