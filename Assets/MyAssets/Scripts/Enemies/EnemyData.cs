using UnityEngine;

namespace MonsterSurvivors
{
    [CreateAssetMenu(fileName = "New Enemy", menuName = "MonsterSurvivors/Enemy Data")]
    public class EnemyData : ScriptableObject
    {
        public string enemyId;
        public string enemyName;
        public GameObject prefab;   //敌人预制体
        public Sprite sprite;       //敌人精灵

        public float hp = 50;
        public float speed = 2f;
        public float damage = 10f;  //碰撞伤害
        public int expReward = 5;   //击杀经验

        // 掉落表
        public EnemyDropEntry[] drops;
    }

    // 单个掉落条目（类型 + 概率）
    [System.Serializable]
    public class EnemyDropEntry
    {
        public DropType dropType;
        [Range(0, 100)] public float chance = 100;
    }
}