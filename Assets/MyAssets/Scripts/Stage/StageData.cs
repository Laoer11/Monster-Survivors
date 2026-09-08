using UnityEngine;

namespace MonsterSurvivors
{
    [CreateAssetMenu(fileName = "New Stage", menuName = "MonsterSurvivors/Stage Data")]
    public class StageData : ScriptableObject
    {
        // 关卡基本信息
        [Header("基本信息")]
        public string stageId;           // 唯一标识
        public string stageName;         // 显示名称 "第一关"
        public string description;       // 描述
        public Sprite previewSprite;     // 关卡预览图
        public string sceneName;         // 加载的场景名（目前都是 "Game"）
        public bool isUnlocked;          // 是否解锁

        // 关卡战斗配置
        [Header("战斗配置")]
        public float duration = 60f;           // 关卡时长（秒）
        public float enemyHPMultiplier = 1f;   // 敌人血量倍率
        public float enemyDamageMultiplier = 1f; // 敌人伤害倍率
        public float enemySpeedMultiplier = 1f;  // 敌人速度倍率

    }
}
