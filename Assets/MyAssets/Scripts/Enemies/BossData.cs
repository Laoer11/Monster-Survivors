using UnityEngine;

namespace MonsterSurvivors
{
    [CreateAssetMenu(fileName = "New Boss", menuName = "MonsterSurvivors/Boss Data")]
    public class BossData : ScriptableObject
    {
        public string bossId;
        public string bossName;
        public GameObject prefab;
        public Sprite sprite;

        [Header("属性")]
        public float hp = 500f;
        public float speed = 1.5f;
        public float damage = 20f;

        [Header("阶段(HP 低于百分比时进入下一阶段)")]
        public float phase2Threshold = 0.5f;    // 50% HP进入狂暴
        public float phase2SpeedMultiplier = 1.5f;
    }
}