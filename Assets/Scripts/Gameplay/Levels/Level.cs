using Commons.ScriptableObjects;
using Generator;
using UnityEngine;

namespace Gameplay.Levels
{
    [CreateAssetMenu(menuName = "Levels/Level")]
    public class Level : GUIDScriptableObject
    {
        [field: SerializeField] public GeneratorConfig GeneratorConfig { get; private set; }

        protected override void Validate()
        {
            if (GeneratorConfig.MigrateCustomStrategyIfNull())
            {
#if UNITY_EDITOR
                UnityEditor.EditorUtility.SetDirty(this);
#endif
            }
        }
    }
}
