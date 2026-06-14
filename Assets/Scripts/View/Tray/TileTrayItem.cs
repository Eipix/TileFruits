using Commons.Extensions;
using DG.Tweening;
using Gameplay;
using UnityEngine;
using UnityEngine.UI;
using View.Animations;
using Zenject;

namespace UI.Tray
{
    public class TileTrayItem : MonoBehaviour
    {
        [SerializeField] private RectTransform _iconsParent;
        [SerializeField] private Image _icon;
        
        private CollectAnimationConfig _collectAnimationConfig;
        private HideAnimationConfig _hideConfig;
        private RectTransform _outerLayoutTransform;

        public RectTransform IconsParent => _iconsParent;
        public RectTransform RectTransform { get; private set; }
        public TileConfig Config { get; private set; }
        
        public Tween Shifting { get; private set; }
        public Sequence ReturningToTray { get; private set; }
        public Tween Hiding { get; private set; }
        
        [Inject]
        private void Construct(CollectAnimationConfig config, HideAnimationConfig hideConfig)
        {
            _collectAnimationConfig = config;
            _hideConfig = hideConfig;
            _outerLayoutTransform = transform.parent.parent as RectTransform;
        }
        
        public void Awake() => RectTransform = transform as RectTransform;

        public Tween ShiftTo(float targetX, float duration, Ease ease)
        {
            Shifting?.Kill();
            Shifting = RectTransform.DOAnchorPosX(targetX, duration)
                .SetEase(ease);

            return Shifting;
        }
        
        public void SetWorldPosition(Vector2 worldPosition)
        {
            IconsParent.SetParent(_outerLayoutTransform, false);
            IconsParent.position = worldPosition;
            IconsParent.SetParent(RectTransform, true);
        }
        
        public Sequence ReturnToTray()
        {
            ReturningToTray.CompleteIfActive(true);
            
            ReturningToTray = DOTween.Sequence()
                .Append(IconsParent.DOAnchorPos(Vector2.zero, _collectAnimationConfig.MoveDuration)
                    .SetEase(_collectAnimationConfig.MoveEase))
                .Append(IconsParent.DOPunchScale(
                    _collectAnimationConfig.Punch, _collectAnimationConfig.PunchDuration,
                    _collectAnimationConfig.Vibrato, _collectAnimationConfig.Elasticity));
            
            return ReturningToTray;
        }

        public Tween Hide()
        {
            Hiding = RectTransform.DOScale(Vector3.zero, _hideConfig.Duration)
                .SetEase(_hideConfig.Ease);

            return Hiding;
        }
        
        public class Pool : MonoMemoryPool<TileConfig, RectTransform, TileTrayItem>
        {
            protected override void Reinitialize(TileConfig config, RectTransform parent, TileTrayItem item)
            {
                base.Reinitialize(config, parent, item);
                
                item.Config = config;
                item._icon.sprite = config.Symbol;
                item.RectTransform.SetParent(parent);
                item.RectTransform.localScale = Vector3.one;
                item.RectTransform.anchoredPosition3D = Vector3.zero;
                item._iconsParent.anchoredPosition3D = Vector3.zero;
            }

            protected override void OnDespawned(TileTrayItem item)
            {
                base.OnDespawned(item);
                
                DOTween.Kill(item);
                DOTween.Kill(item.RectTransform);
                DOTween.Kill(item.IconsParent);
                
                item.ReturningToTray?.Kill();
                item.RectTransform.localScale = Vector3.one;
                item.RectTransform.anchoredPosition3D = Vector3.zero;
                item._iconsParent.anchoredPosition3D = Vector3.zero;
            }
        }
    }
}
