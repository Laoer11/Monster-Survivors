using UnityEngine;

namespace MonsterSurvivors
{
    /// <summary>
    /// 角色数据定义 —— ScriptableObject
    /// </summary>
    [CreateAssetMenu(fileName = "New Character", menuName = "MonsterSurvivors/Character Data")]
    public class CharacterData : ScriptableObject
    {
        public string characterId;              // 唯一 ID
        public string characterName;            // 显示名称 如"法师"
        public string description;              // 描述文字
        public Sprite icon;                     // 角色图标

        [Header("基础属性")]
        public float baseHP = 100f;
        public float baseDamage = 10f;
        public float baseSpeed = 5f;
        public float baseMagnetRadius = 2f;

        [Header("倍率初始值")]
        public float defaultXPMultiplier = 1f;
        public float defaultCooldownMultiplier = 1f;
        public float defaultProjectileSpeed = 1f;
        public float defaultSizeMultiplier = 1f;
        public float defaultDurationMultiplier = 1f;
        public float defaultGoldMultiplier = 1f;

        [Header("免伤")]
        [Range(0,100)] public int initialDamageReductionPercent = 0;

        [Header("角色动画预制体")]
        public GameObject characterPrefab;     // 暂时可以留空

    }
}