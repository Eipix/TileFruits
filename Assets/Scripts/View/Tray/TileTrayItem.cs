using Commons.Extensions;
using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using Gameplay;
using UnityEngine;
using UnityEngine.UI;
using View;
using View.Animations;
using View.Effects;
using Zenject;

namespace UI.Tray
{
    public class TileTrayItem : MonoBehaviour
    {
        [SerializeField] private float _baseTrailWidth;
        [SerializeField] private TrailRenderer _trailRenderer;
        [SerializeField] private RectTransform _iconsParent;
        [SerializeField] private Image _icon;
        
        private CollectAnimationConfig _collectAnimationConfig;
        private HideAnimationConfig _hideConfig;
        private MatchEffect.Pool _matchEffectPool;
        private RectTransform _outerLayoutTransform;
        private CanvasSizeTracker _canvasSizeTracker;
        
        private Vector2 _initialScale;

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
            
            RectTransform = transform as RectTransform;
            _outerLayoutTransform = transform.parent.parent as RectTransform;
            _initialScale = _iconsParent.localScale;
            
            _trailRenderer.emitting = false;
            _canvasSizeTracker = _icon.canvas.GetComponent<CanvasSizeTracker>();
        }

        private void OnEnable() => _canvasSizeTracker.Changed += ResizeTrail;

        private void OnDisable() => _canvasSizeTracker.Changed -= ResizeTrail;

        private void ResizeTrail()
        {
            var canvasRect = _canvasSizeTracker.RectTransform;
            var referenceResolution = _canvasSizeTracker.ReferenceResolution;
            
            float canvasScaleFactor = (canvasRect.sizeDelta / referenceResolution).x;
            
            float elementScale = _iconsParent.localScale.x;

            _trailRenderer.widthMultiplier = _baseTrailWidth * canvasScaleFactor * elementScale;
        }

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
            _iconsParent.anchoredPosition3D *= Vector2.one;
        }
        
        public Sequence ReturnToTray(Vector2 startScale)
        {
            ReturningToTray.CompleteIfActive(true);

            _trailRenderer.Clear();
            _trailRenderer.emitting = true;
            _iconsParent.localScale = startScale;
            
            ReturningToTray = DOTween.Sequence()
                .Append(DOPunchScale())
                .Append(_iconsParent.DOAnchorPos(Vector2.zero, _collectAnimationConfig.MoveDuration)
                    .SetEase(_collectAnimationConfig.MoveEase))
                .Join(DOScale(_initialScale, _collectAnimationConfig.MoveDuration))
                .AppendCallback(() => _trailRenderer.emitting = false)
                .Append(DOPunchScale());
            
            return ReturningToTray;
        }

        public Tween Hide()
        {
            _matchEffectPool.Spawn(transform.position);
            
            Hiding = RectTransform.DOScale(Vector3.zero, _hideConfig.Duration)
                .SetEase(_hideConfig.Ease);

            return Hiding;
        }

        private TweenerCore<Vector3, Vector3, VectorOptions> DOScale(Vector3 target, float duration)
        {
            return DOTween.To(
                () => _iconsParent.localScale,
                value =>
                {
                    _iconsParent.localScale = value;
                    ResizeTrail();
                }, target, duration);
        }

        private Tween DOPunchScale() => _iconsParent.DOPunchScale(
                _collectAnimationConfig.Punch, _collectAnimationConfig.PunchDuration,
                _collectAnimationConfig.Vibrato, _collectAnimationConfig.Elasticity);

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
