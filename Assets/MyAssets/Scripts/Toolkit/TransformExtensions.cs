using System.Collections;
using UnityEngine;

namespace MonsterSurvivors
{
    /// <summary>
    /// Transform 的缓动动画扩展方法
    /// </summary>
    public static class TransformExtensions
    {
        /// <summary>
        /// 平滑移动到位移目标
        /// </summary>
        public static EasingCoroutine DoMove(this Transform transform, Vector3 target, float duration,
            EasingType easing = EasingType.SineOut)
        {
            return EasingManager.Instance.RunAnimation(
                MoveCoroutine(transform, target, duration, easing));
        }

        private static IEnumerator MoveCoroutine(Transform transform, Vector3 target,
            float duration, EasingType easing)
        {
            Vector3 start = transform.position;
            float elapsed = 0f;

            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                float t = EasingFunctions.Evaluate(easing, elapsed / duration);
                transform.position = Vector3.LerpUnclamped(start, target, t);
                yield return null;   // 等下一帧
            }

            if (transform == null) yield break;

            transform.position = target;  // 确保精确到达
        }

        /// <summary>
        /// 平滑缩放
        /// </summary>
        public static EasingCoroutine DoScale(this Transform transform, Vector3 target, float duration,
            EasingType easing = EasingType.BackOut)
        {
            return EasingManager.Instance.RunAnimation(
                ScaleCoroutine(transform, target, duration, easing));
        }

        private static IEnumerator ScaleCoroutine(Transform transform, Vector3 target,
            float duration, EasingType easing)
        {
            Vector3 start = transform.localScale;
            float elapsed = 0f;

            while (elapsed < duration)
            {
                if (transform == null) yield break;

                elapsed += Time.deltaTime;
                float t = EasingFunctions.Evaluate(easing, elapsed / duration);
                transform.localScale = Vector3.LerpUnclamped(start, target, t);
                yield return null;
            }

            if (transform == null) yield break;

            transform.localScale = target;
        }

        /// <summary>
        /// 弹跳弹出（从 0 到 1，常用于 UI 弹窗出现）
        /// </summary>
        public static EasingCoroutine DoPopIn(this Transform transform, float duration = 0.3f)
        {
            transform.localScale = Vector3.zero;
            return transform.DoScale(Vector3.one, duration, EasingType.BackOut);
        }

        /// <summary>
        /// 缩小消失（从 1 到 0，常用于 UI 弹窗关闭）
        /// </summary>
        public static EasingCoroutine DoPopOut(this Transform transform, float duration = 0.2f)
        {
            return transform.DoScale(Vector3.zero, duration, EasingType.ExpoIn);
        }
    }
}