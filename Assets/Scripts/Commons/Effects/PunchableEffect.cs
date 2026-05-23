using Commons.Extensions;
using UnityEngine;
using DG.Tweening;
using UnityEngine.EventSystems;

public class PunchableEffect : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    [SerializeField] private bool _useIndependentUpdate = true;
    [SerializeField, Range(0, 1)] private float _strength;
    [SerializeField, Range(0, 1)] private float _duration;

    private Tween _punch;
    private Vector3 _defaultScale;

    private void Awake() => _defaultScale = transform.localScale;

    private void OnDestroy() => _punch?.Kill();

    public void OnPointerDown(PointerEventData eventData) => ScaleDown();

    public void OnPointerUp(PointerEventData eventData) => ScaleUp();

    private void ScaleDown() => Scale(_defaultScale * (1 - _strength));

    private void ScaleUp() => Scale(_defaultScale);

    private void Scale(Vector3 targetScale)
    {
        _punch?.CompleteIfActive();
        _punch = transform.DOScale(targetScale, _duration)
            .SetUpdate(_useIndependentUpdate);
    }
}
