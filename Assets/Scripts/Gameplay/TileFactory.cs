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
        {
            return Create(config, position, Quaternion.identity, parent);
        }

        public Tile Create(TileConfig config, Vector2 position, Quaternion rotation, Transform parent)
        {
            return _instantiator.InstantiatePrefabForComponent<Tile>(
                _prefab, 
                position, 
                rotation,
                parent, 
                extraArgs: new object[] { config }
            );
        }
    }
}
