using NaughtyAttributes;
using UnityEngine;
using Zenject;

namespace Gameplay
{
    public class Tile : MonoBehaviour, IInitializable
    {
        [SerializeField] private SpriteRenderer _bone;
        [SerializeField] private SpriteRenderer _symbol;

        private TileConfig _config;
        
        private int _boneInitialSortingOrder;
        private int _symbolInitialSortingOrder;

        [Inject]
        public void Construct(TileConfig config)
        {
            _config = config;
        }

        public void Initialize()
        {
            _symbol.sprite = _config.Symbol;
            _boneInitialSortingOrder = _bone.sortingOrder;
            _symbolInitialSortingOrder = _symbol.sortingOrder;
        }
        
        [Button(enabledMode: EButtonEnableMode.Playmode)]
        public void LayerUp()
        {
            _bone.sortingOrder++;
            _symbol.sortingOrder++;
        }
        
        [Button(enabledMode: EButtonEnableMode.Playmode)]
        public void LayerDown()
        {
            _bone.sortingOrder--;
            _symbol.sortingOrder--;
        }
        
        public void ResetLayers()
        {
            _bone.sortingOrder = _boneInitialSortingOrder;
            _symbol.sortingOrder = _symbolInitialSortingOrder;
        }
    }
}
