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
        public void Construct(TileConfig config, int layer)
        {
            _config = config;
            SetLayer(layer);
        }

        public void Initialize()
        {
            _symbol.sprite = _config.Symbol;
            _boneInitialSortingOrder = _bone.sortingOrder;
            _symbolInitialSortingOrder = _symbol.sortingOrder;
        }

        private void SetLayer(int layer)
        {
            _bone.sortingOrder = layer;
            _symbol.sortingOrder = layer;
        }
        
        [Button(enabledMode: EButtonEnableMode.Playmode)]
        private void LayerUp()
        {
            _bone.sortingOrder++;
            _symbol.sortingOrder++;
        }
        
        [Button(enabledMode: EButtonEnableMode.Playmode)]
        private void LayerDown()
        {
            _bone.sortingOrder--;
            _symbol.sortingOrder--;
        }
        
        [Button(enabledMode: EButtonEnableMode.Playmode)]
        private void ResetLayers()
        {
            _bone.sortingOrder = _boneInitialSortingOrder;
            _symbol.sortingOrder = _symbolInitialSortingOrder;
        }
    }
}
