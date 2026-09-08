using UnityEngine;

namespace MonsterSurvivors
{
    public static class ColorExtensions
    {
        /// <summary>
        /// 返回只改了 alpha 的新颜色
        /// </summary>
        public static Color SetAlpha(this Color c, float a) => new Color(c.r, c.g, c.b, a);
    }
}
