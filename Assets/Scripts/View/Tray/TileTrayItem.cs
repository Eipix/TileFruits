using Coffee.UIExtensions;
using Commons.Extensions;
using DG.Tweening;
using Gameplay;
using UnityEngine;
using UnityEngine.UI;
using View.Animations;
using View.Effects;
using Zenject;

namespace UI.Tray
{
    public class TileTrayItem : MonoBehaviour
    {
        [SerializeField] private TrailRenderer _trailRenderer;
        [SerializeField] private RectTransform _iconsParent;
        [SerializeField] private Image _icon;
        
        private CollectAnimationConfig _collectAnimationConfig;
        private HideAnimationConfig _hideConfig;
        private MatchEffect.Pool _matchEffectPool;
        private RectTransform _outerLayoutTransform;
        
        private Vector2 _initialScale;
        private float _initialTrailStartWidth;

        public RectTransform RectTransform { get; private set; }
        public TileConfig Config { get; private set; }
        
        public Tween Shifting { get; private set; }
        public Sequence ReturningToTray { get; private set; }
        public Tween Hiding { get; private set; }
        
        [Inject]
        private void Construct(CollectAnimationConfig config,
            HideAnimationConfig hideConfig,
            MatchEffect.Pool matchEffectPool)
        {
            _collectAnimationConfig = config;
            _hideConfig = hideConfig;
            _matchEffectPool = matchEffectPool;
            
            _outerLayoutTransform = transform.parent.parent as RectTransform;
            _initialScale = _iconsParent.localScale;
            _trailRenderer.emitting = false;
            _initialTrailStartWidth = _trailRenderer.startWidth;
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
            _iconsParent.SetParent(_outerLayoutTransform, false);
            _iconsParent.position = worldPosition;
            _iconsParent.SetParent(RectTransform, true);
        }
        
        public Sequence ReturnToTray(Vector2 startScale)
        {
            ReturningToTray.CompleteIfActive(true);

            _trailRenderer.emitting = true;

            ReturningToTray = DOTween.Sequence()
                .OnStart(() =>
                {
                    _iconsParent.localScale = startScale;
                    _trailRenderer.startWidth *= Mathf.Max(startScale.x, startScale.y);
                })
                .Append(_iconsParent.DOAnchorPos(Vector2.zero, _collectAnimationConfig.MoveDuration)
                    .SetEase(_collectAnimationConfig.MoveEase))
                .Join(_iconsParent.DOScale(_initialScale, _collectAnimationConfig.MoveDuration))
                .Join(_trailRenderer.DOResize(_initialTrailStartWidth, _trailRenderer.endWidth,
                    _collectAnimationConfig.MoveDuration))
                .AppendCallback(() => _trailRenderer.emitting = false)
                .Append(_iconsParent.DOPunchScale(
                    _collectAnimationConfig.Punch, _collectAnimationConfig.PunchDuration,
                    _collectAnimationConfig.Vibrato, _collectAnimationConfig.Elasticity));
            
            return ReturningToTray;
        }

        public Tween Hide()
        {
            _matchEffectPool.Spawn(transform.position);
            
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
                DOTween.Kill(item._iconsParent);
                
                item.ReturningToTray?.Kill();
                
                item.RectTransform.localScale = Vector3.one;
                item.RectTransform.anchoredPosition3D = Vector3.zero;
                item._iconsParent.anchoredPosition3D = Vector3.zero;
            }
        }
    }
}
