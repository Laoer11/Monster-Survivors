using UnityEngine;

namespace MonsterSurvivors
{
    public static class VectorExtensions
    {
        /// <summary>
        /// 返回只改了 x 的新 Vector3
        /// </summary>
        public static Vector3 SetX(this Vector3 v, float x) => new Vector3(x, v.y, v.z);

        /// <summary>
        /// 返回只改了 y 的新 Vector3
        /// </summary>
        public static Vector3 SetY(this Vector3 v, float y) => new Vector3(v.x, y, v.z);

        /// <summary>
        /// 返回只改了 z 的新 Vector3
        /// </summary>
        public static Vector3 SetZ(this Vector3 v, float z) => new Vector3(v.x, v.y, z);

        /// <summary>
        /// 返回 Vector3 的 (x, y) 部分作为 Vector2
        /// </summary>
        public static Vector2 XY(this Vector3 v) => new Vector2(v.x, v.y);
    }
}
