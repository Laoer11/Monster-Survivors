using System.Collections.Generic;
using UnityEngine;

namespace MonsterSurvivors
{
    public enum AbilityType
    {
        // 武器
        Fireball,

        // 主动
        IceShard,
        Lightning,
        
        // 被动
        MoveSpeed,
        Damage,
        MaxHP,
        Cooldown,

        // 进化
        RapidFireball,   // 快速火球 = 火球 + 冷却
    }

    [System.Serializable]
    public class EvolutionRequirement
    {
        public AbilityType abilityType;     // 需要哪个技能
        public int requiredLevel = 4;       // 需要升到多少级（4 = 满级）
    }

    [CreateAssetMenu(fileName = "New Ability", menuName = "MonsterSurvivors/Ability Data")]
    public class AbilityData : ScriptableObject
    {
        public AbilityType abilityType;
        public string abilityName;          // 如"火球术"
        public string description;          // 如"向目标发射一球火球，造成伤害"
        public Sprite icon;                 // 技能图标

        public bool isWeaponAbility;        // 是武器技能（开局必选一个）
        public bool isActiveAbility;        // 是主动技能
        // isWeapon 和 isActive 都为 false = 被动技能

        public int maxLevel = 5;            // 最高等级
        public GameObject abilityPrefab;    // 技能预制体（挂 AbilityBehavior）

        [Header("进化")]
        public bool isEvolution;            // 自身是进化技能吗？
        public List<EvolutionRequirement> evolutionRequirements;    
    }
}