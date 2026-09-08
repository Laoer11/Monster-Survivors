using System.Collections.Generic;
using UnityEngine;

namespace MonsterSurvivors
{
    [CreateAssetMenu(fileName = "AbilitiesDatabase", menuName = "MonsterSurvivors/Abilities Database")]
    public class AbilitiesDatabase : ScriptableObject
    {
        public List<AbilityData> abilities = new List<AbilityData>();

        public int activeSlotCount = 6; // 主动技能槽位数量最多 6 个
        public int passiveSlotCount = 4; // 被动技能槽位数量最多 4 个

        [Header("选择权重")]
        [Range(0,10)] public float existingAbilityWeight = 5; // 已有技能升级的权重
        [Range(0,10)] public float newAbilityWeight = 1f;     // 新技能的权重

        public AbilityData GetByType(AbilityType type)
        {
            return abilities.Find(a => a.abilityType == type);
        }

        public int Count => abilities.Count;
    }
}