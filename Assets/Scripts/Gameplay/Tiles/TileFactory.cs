using Gameplay;
using UnityEngine;
using Zenject;

namespace Generator
{
    public class TileFactory
    {
        private readonly IInstantiator _instantiator;
        private readonly Tile _prefab;
        
        public TileFactory(IInstantiator instantiator, Tile prefab)
        {
            _instantiator = instantiator;
            _prefab = prefab;
        }

        public Tile Create(TileConfig config, Vector3 position, Transform parent)
            => Create(config, position, parent, 0);

        public Tile Create(TileConfig config, Vector2 position, Transform parent, int layer)
        {
            return _instantiator.InstantiatePrefabForComponent<Tile>(
                _prefab, 
                position, 
                Quaternion.identity,
                parent, 
                extraArgs: new object[] { config, layer }
            );
        }
    }
}
