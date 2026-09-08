using System;

namespace MonsterSurvivors
{
    /// <summary>
    /// 玩家存档数据示例
    /// 用 [Serializable] 标记，表示可以被 JSON 序列化
    /// </summary>
    [Serializable]
    public class PlayerSave : ISave
    {
        // ===== 运行时数据（游戏中使用） =====
        public int gold;              // 当前金币
        public int maxReachedStage;   // 最高通关关卡编号
        public int totalEnemiesKilled; // 累计击杀数

        // ===== ISave 实现 =====
        public void Init()
        {
            gold = 0;
            maxReachedStage = 0;
            totalEnemiesKilled = 0;
        }

        public void Flush()
        {
            // 保存运行时数据到存档对象
        }
    }
}