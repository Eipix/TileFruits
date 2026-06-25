using _Commons.Scripts.Effects.Shakers;
using Commons.Extensions;
using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;
using Zenject;

namespace View.Animations
{
    public class CollectionItemShaker : MonoBehaviour, IPointerDownHandler
    {
        [Inject] private ShakerConfig _config;
        
        private Tween _shaking;

        public void OnPointerDown(PointerEventData eventData) =>
            Shake();

        private void Shake()
        {
            _shaking.CompleteIfActive();
            _shaking = transform.DOShakePosition(_config);
        }
    }
}
