using System;
using System.Collections;
using System.Linq;
using JetBrains.Annotations;
using UnityEngine;

namespace Commons.Extensions
{
    public static class AnimatorExtensions
    {
        public static IEnumerator WaitNormalizedTime(this Animator animator, float targetNormalizedTime,
            Action onTargetAchieved = null)
        {
            yield return WaitNormalizedTime(animator, 0, targetNormalizedTime, onTargetAchieved);
        }

        public static IEnumerator WaitNormalizedTime(this Animator animator, int layerIndex, float targetNormalizedTime,
            Action onTargetAchieved = null)
        {
            targetNormalizedTime = Mathf.Clamp01(targetNormalizedTime);

            yield return new WaitUntil(() =>
                animator.GetCurrentAnimatorStateInfo(layerIndex).normalizedTime >= targetNormalizedTime);

            onTargetAchieved?.Invoke();
        }

        public static bool HasEvent(this Animator animator, string eventName)
        {
            var clips = animator.runtimeAnimatorController.animationClips;
            int clipsLength = clips.Length;

            for (int i = 0; i < clipsLength; i++)
            {
                if (clips[i].events.Any(e => e.functionName == eventName))
                {
                    return true;
                }
            }

            return false;
        }

        public static bool IsGoingToState(this Animator animator, string stateName, int layerIndex = 0)
        {
            if (animator.IsInTransition(layerIndex))
            {
                return animator.GetNextAnimatorStateInfo(layerIndex).IsName(stateName);
            }

            return animator.GetCurrentAnimatorStateInfo(layerIndex).IsName(stateName);
        }

        public static bool HasState(this Animator animator, string stateName)
        {
            return HasState(animator, 0, stateName);
        }

        public static bool HasState(this Animator animator, int layerIndex, string stateName)
        {
            int stateHash = Animator.StringToHash(stateName);
            return animator.HasState(layerIndex, stateHash);
        }


        public static IEnumerator PlayAndWait(this Animator animator, string stateName, Action onComplete = null)
        {
            yield return PlayAndWait(animator, stateName, 0, onComplete);
        }

        public static IEnumerator PlayAndWait(this Animator animator, string stateName, int layerIndex,
            Action onComplete = null)
        {
            animator.Play(stateName, layerIndex, 0f);
            yield return WaitForStart(animator, stateName, layerIndex);
            yield return WaitForCompletion(animator, stateName, layerIndex);
            onComplete?.Invoke();
        }

        public static IEnumerator CrossFadeInFixedTimeAndWait(this Animator animator, string stateName, float duration,
            [CanBeNull] Action onComplete = null)
        {
            return animator.CrossFadeInFixedTimeAndWait(stateName, duration, 0, onComplete);
        }

        public static IEnumerator CrossFadeInFixedTimeAndWait(this Animator animator, string stateName, float duration,
            int layerIndex, [CanBeNull] Action onComplete = null)
        {
            animator.CrossFadeInFixedTime(stateName, duration, layerIndex, 0f);
            yield return WaitForStart(animator, stateName, layerIndex);
            yield return WaitForCompletion(animator, stateName, layerIndex);
            onComplete?.Invoke();
        }

        public static IEnumerator CrossFadeAndWait(this Animator animator, string stateName, float duration,
            int layerIndex = 0, [CanBeNull] Action onComplete = null)
        {
            animator.CrossFade(stateName, duration, layerIndex, 0f);
            yield return WaitForStart(animator, stateName, layerIndex);
            yield return WaitForCompletion(animator, stateName, layerIndex);
            onComplete?.Invoke();
        }

        public static IEnumerator WaitForStart(this Animator animator, string stateName, int layerIndex = 0)
        {
            yield return null;

            while (true)
            {
                var nextInfo = animator.GetNextAnimatorStateInfo(layerIndex);
                bool isInTransition = animator.IsInTransition(layerIndex);

                if (isInTransition && nextInfo.IsName(stateName))
                {
                    yield return null;
                    continue;
                }

                break;
            }
        }

        public static IEnumerator WaitForCompletion(this Animator animator, string stateName, int layerIndex = 0)
        {
            while (true)
            {
                var currentInfo = animator.GetCurrentAnimatorStateInfo(layerIndex);
                var nextInfo = animator.GetNextAnimatorStateInfo(layerIndex);
                bool isInTransition = animator.IsInTransition(layerIndex);

                if (isInTransition && nextInfo.IsName(stateName))
                {
                    yield return null;
                    continue;
                }

                if (!isInTransition && currentInfo.IsName(stateName))
                {
                    if (currentInfo.normalizedTime >= 0.99f)
                    {
                        break;
                    }
                }
                else if (isInTransition && currentInfo.IsName(stateName))
                {
                    break;
                }
                else if (!currentInfo.IsName(stateName) && !nextInfo.IsName(stateName))
                {
                    break;
                }

                yield return null;
            }
        }
    }
}
