using UnityEngine;

namespace Commons.Extensions
{
    public static class CanvasGroupExtensions
    {
        public static void Show(this CanvasGroup canvasGroup) => SetActive(canvasGroup, true);

        public static void Hide(this CanvasGroup canvasGroup) => SetActive(canvasGroup, false);

        public static void SetActive(this CanvasGroup canvasGroup, bool active)
        {
            canvasGroup.interactable = active;
            canvasGroup.blocksRaycasts = active;
            canvasGroup.alpha = active ? 1 : 0;
        }
    }
}
