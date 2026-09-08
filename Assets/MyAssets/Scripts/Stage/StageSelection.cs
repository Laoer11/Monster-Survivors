using UnityEngine;

namespace MonsterSurvivors
{
    /// <summary>
    /// 静态存储当前选中的关卡
    /// </summary>
    public static class StageSelection
    {
        public static StageData CurrentStage { get; set; }
    }
}