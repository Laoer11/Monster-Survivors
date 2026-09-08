using System.Collections;
using UnityEngine;

namespace MonsterSurvivors
{
    /// <summary>
    /// SpriteRenderer 的缓动扩展
    /// </summary>
    public static class SpriteRendererExtensions
    {
        /// <summary>
        /// 平滑淡入/淡出（改变 alpha）
        /// </summary>
        public static EasingCoroutine DoAlpha(this SpriteRenderer sr, float targetAlpha,
            float duration, EasingType easing = EasingType.SineOut)
        {
            return EasingManager.Instance.RunAnimation(
                AlphaCoroutine(sr, targetAlpha, duration, easing));
        }

        private static IEnumerator AlphaCoroutine(SpriteRenderer sr, float target,
            float duration, EasingType easing)
        {
            Color color = sr.color;
            float start = color.a;
            float elapsed = 0f;

            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                float t = EasingFunctions.Evaluate(easing, elapsed / duration);
                color.a = Mathf.LerpUnclamped(start, target, t);
                sr.color = color;
                yield return null;
            }

            color.a = target;
            sr.color = color;
        }

        /// <summary>
        /// 平滑变色（如受伤闪红）
        /// </summary>
        public static EasingCoroutine DoColor(this SpriteRenderer sr, Color targetColor,
            float duration, EasingType easing = EasingType.SineOut)
        {
            return EasingManager.Instance.RunAnimation(
                ColorCoroutine(sr, targetColor, duration, easing));
        }

        private static IEnumerator ColorCoroutine(SpriteRenderer sr, Color target,
            float duration, EasingType easing)
        {
            Color start = sr.color;
            float elapsed = 0f;

            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                float t = EasingFunctions.Evaluate(easing, elapsed / duration);
                sr.color = Color.Lerp(start, target, t);
                yield return null;
            }

            sr.color = target;
        }

        /// <summary>
        /// 直接设置 alpha（非动画）
        /// </summary>
        public static void SetAlpha(this SpriteRenderer sr, float alpha)
        {
            Color c = sr.color;
            c.a = alpha;
            sr.color = c;
        }
    }
}
