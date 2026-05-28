using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

namespace Input.Levels
{
    [CreateAssetMenu(menuName = "Levels/Level List")]
    public class LevelList : ScriptableObjectInstaller, IEnumerable<Level>
    {
        [SerializeField] private List<Level> _configs;

        public Level this[int index] => _configs[index];
        
        public IEnumerator<Level> GetEnumerator() => _configs.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        public override void InstallBindings()
            => Container.Bind<LevelList>().FromInstance(this).AsSingle().NonLazy();
    }
}
