using UnityEngine;

namespace MonsterSurvivors
{
    /// <summary>
    /// 升级类型枚举
    /// </summary>
    public enum UpgradeType
    {
        Damage,     // 攻击力
        Health,     // 最大生命
        Armor,      // 减伤
        Healing,    // 回复量
        Revive,     // 复活次数
    }

    /// <summary>
    /// 单个升级项的定义 —— ScriptableObject
    /// </summary>
    [CreateAssetMenu(fileName = "New Upgrade", menuName = "MonsterSurvivors/Upgrade Data")]
    public class UpgradeData : ScriptableObject
    {
        public UpgradeType upgradeType;        // 升级类型
        public string upgradeName;             // 显示名称 "攻击力提升"
        public string description;             // 描述
        public Sprite icon;                    // 图标

        [Header("数值")]
        public int maxLevel = 5;               // 最高等级
        public float valuePerLevel = 10f;      // 每级加多少

        [Header("花费")]
        public int[] costs;                    // 每级花费（长度 = maxLevel）

        /// <summary>
        /// 获取当前等级对应的属性加成
        /// </summary>
        public float GetValue(int level)
        {
            return valuePerLevel * level;
        }

        /// <summary>
        /// 获取升到下一级所需金币
        /// </summary>
        public int GetCost(int currentLevel)
        {
            if (currentLevel >= maxLevel) return -1;
            return costs[currentLevel];
        }
    }
}
