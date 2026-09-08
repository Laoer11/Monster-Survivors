namespace MonsterSurvivors
{
    public enum EasingType
    {
        /// <summary>
        /// 缓动类型枚举 —— 决定动画的"节奏感"
        /// </summary>
            Linear,

            // Sine 系列：最柔和，适合 UI 淡入淡出
            SineIn,
            SineOut,
            SineInOut,

            // Quad 系列：比 Sine 稍快，适合小物体移动
            QuadIn,
            QuadOut,
            QuadInOut,

            // Expo 系列：非常快，适合"弹出"效果
            ExpoIn,
            ExpoOut,
            ExpoInOut,

            // Back 系列：会"冲过头再回来"，适合弹窗
            BackIn,
            BackOut,
            BackInOut,

            // Elastic：橡皮筋弹性
            ElasticOut,

            // Bounce：弹跳，适合落地
            BounceOut,

    }
}