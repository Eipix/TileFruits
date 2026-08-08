using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Commons.Extensions;
using NaughtyAttributes;
using UnityEngine;
using Zenject;

namespace Gameplay
{
    [CreateAssetMenu(menuName = "Tiles/TileList")]
    public class TileList : ScriptableObjectInstaller, IEnumerable<TileConfig>
    {
        private const string DuplicateNameMessage = "Null or duplicate tiles are not allowed";

        [SerializeField, ValidateInput(nameof(Validate), DuplicateNameMessage)]
        private List<TileConfig> _configs;

        public int Length => _configs.Count;
        
        public TileConfig this[int index] => _configs[index];

        private bool Validate()
        {
            if (_configs == null)
                return false;

            bool hasIdDuplicate = _configs.HasDuplicate(config => config.Id);
            bool hasSymbolDuplicate = _configs.HasDuplicate(config => config.Symbol);
            
            return (hasIdDuplicate || hasSymbolDuplicate) is false;
        }

        public IEnumerator<TileConfig> GetEnumerator() => _configs.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        public override void InstallBindings() => Container.BindInstance(this).AsSingle();
    }
}
