using UnityEngine;

namespace MonsterSurvivors
{
    /// <summary>
    /// 缓动函数库 —— 输入 t ∈ [0, 1]，输出经过缓动的值 ∈ [0, 1]
    /// 所有公式来源于 https://easings.net
    /// </summary>
    public static class EasingFunctions
    {
        /// <summary>
        /// 根据类型计算缓动值
        /// </summary>
        public static float Evaluate(EasingType type, float t)
        {
            t = Mathf.Clamp01(t);
            return type switch
            {
                EasingType.Linear => t,

                EasingType.SineIn => 1f - Mathf.Cos(t * Mathf.PI / 2f),
                EasingType.SineOut => Mathf.Sin(t * Mathf.PI / 2f),
                EasingType.SineInOut => -(Mathf.Cos(Mathf.PI * t) - 1f) / 2f,

                EasingType.QuadIn => t * t,
                EasingType.QuadOut => 1f - (1f - t) * (1f - t),
                EasingType.QuadInOut => t < 0.5f ? 2f * t * t : 1f - Mathf.Pow(-2f * t + 2f, 2f) / 2f,

                EasingType.ExpoIn => t == 0f ? 0f : Mathf.Pow(2f, 10f * t - 10f),
                EasingType.ExpoOut => t == 1f ? 1f : 1f - Mathf.Pow(2f, -10f * t),
                EasingType.ExpoInOut => t == 0f ? 0f : t == 1f ? 1f : t < 0.5f
                    ? Mathf.Pow(2f, 20f * t - 10f) / 2f
                    : (2f - Mathf.Pow(2f, -20f * t + 10f)) / 2f,

                EasingType.BackIn => 2.70158f * t * t * t - 1.70158f * t * t,
                EasingType.BackOut => 1f + 2.70158f * Mathf.Pow(t - 1f, 3f) + 1.70158f * Mathf.Pow(t - 1f, 2f),
                EasingType.BackInOut => t < 0.5f
                    ? (Mathf.Pow(2f * t, 2f) * ((2.5949f + 1f) * 2f * t - 2.5949f)) / 2f
                    : (Mathf.Pow(2f * t - 2f, 2f) * ((2.5949f + 1f) * (t * 2f - 2f) + 2.5949f) + 2f) / 2f,

                EasingType.ElasticOut => t == 0f ? 0f : t == 1f ? 1f
                    : Mathf.Pow(2f, -10f * t) * Mathf.Sin((t - 0.075f) * (2f * Mathf.PI) / 0.3f) + 1f,

                EasingType.BounceOut => BounceOutImpl(t),

                _ => t,
            };

        }

        private static float BounceOutImpl(float t)
        {
            if (t < 1f / 2.75f) return 7.5625f * t * t;
            if (t < 2f / 2.75f) { t -= 1.5f / 2.75f; return 7.5625f * t * t + 0.75f; }
            if (t < 2.5f / 2.75f) { t -= 2.25f / 2.75f; return 7.5625f * t * t + 0.9375f; }
            t -= 2.625f / 2.75f;
            return 7.5625f * t * t + 0.984375f;
        }
    }
}
