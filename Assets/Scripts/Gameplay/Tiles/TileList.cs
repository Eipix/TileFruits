using System.Collections;
using System.Collections.Generic;
using System.Linq;
using NaughtyAttributes;
using UnityEngine;

namespace Gameplay
{
    [CreateAssetMenu(menuName = "Tiles/TileList")]
    public class TileList : ScriptableObject, IEnumerable<TileConfig>
    {
        private const string DuplicateNameMessage = "Null or duplicate tiles are not allowed";

        [SerializeField, ValidateInput(nameof(Validate), DuplicateNameMessage)]
        private List<TileConfig> _configs;

        public TileConfig this[int index] => _configs[index];

        private bool Validate()
        {
            if (_configs == null)
                return false;

            bool hasIdDuplicate = _configs
                .GroupBy(config => config.Id)
                .Any(g => g.Count() > 1);
            
            bool hasSymbolDuplicate = _configs
                .GroupBy(config => config.Symbol)
                .Any(g => g.Count() > 1);
            
            return (hasIdDuplicate || hasSymbolDuplicate) is false;
        }

        public IEnumerator<TileConfig> GetEnumerator() => _configs.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}
