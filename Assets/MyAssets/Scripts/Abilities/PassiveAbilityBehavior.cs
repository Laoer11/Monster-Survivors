using UnityEngine;

namespace MonsterSurvivors
{
    public class PassiveAbilityBehavior : MonoBehaviour, IAbilityBehavior
    {
        [SerializeField] private AbilityType abilityType;

        [Header("每级加成")]
        [SerializeField] private float[] values = { 1.1f, 1.2f, 1.3f, 1.4f, 1.5f };

        private int currentLevel;

        public void Init(int level)
        {
            currentLevel = level;
            Apply();
        }

        public void Upgrade(int level)
        {
            currentLevel = level;
            Apply();
        }

        private void Apply()
        {
            float value = values[currentLevel];

            switch (abilityType)
            {
                case AbilityType.MoveSpeed:
                    PlayerBehavior.Instance.ApplyMoveSpeedMultiplier(value);
                    break;
                case AbilityType.Damage:
                    PlayerBehavior.Instance.ApplyDamageMultiplier(value);
                    break;
                case AbilityType.MaxHP:
                    PlayerBehavior.Instance.ApplyMaxHPMultiplier(value);
                    break;
                case AbilityType.Cooldown:
                    PlayerBehavior.Instance.ApplyCooldownMultiplier(1f / value);   
                    break;
            }

            Debug.Log($"[Passive] {abilityType} Lv.{currentLevel} 生效, 倍率: {value}");
        }
    }
}