using System.Collections;
using UnityEngine;

namespace MonsterSurvivors
{
    public static class CanvasGroupExtensions
    {
        public static EasingCoroutine DoFade(this CanvasGroup cg, float targetAlpha,
            float duration, EasingType easing = EasingType.SineOut)
        {
            return EasingManager.Instance.RunAnimation(
                FadeCoroutine(cg, targetAlpha, duration, easing));
        }

        private static IEnumerator FadeCoroutine(CanvasGroup cg, float target,
            float duration, EasingType easing)
        {
            float start = cg.alpha;
            float elapsed = 0f;

            while (elapsed < duration)
            {
                elapsed += Time.unscaledDeltaTime;
                cg.alpha = Mathf.LerpUnclamped(start, target,
                    EasingFunctions.Evaluate(easing, elapsed / duration));
                yield return null;
            }
            cg.alpha = target;
        }
    }
}
