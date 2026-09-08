using UnityEngine;

namespace MonsterSurvivors
{
    /// <summary>
    /// 一波敌人的定义
    /// </summary>
    [CreateAssetMenu(fileName = "New Wave", menuName = "MonsterSurvivors/Wave Data")]
    public class WaveData : ScriptableObject
    {
        public EnemyData enemyData;         // 这波出什么敌人
        public int totalCount = 1;          // 这波敌人总数
        public float spawnInterval = 1f;    // 生成间隔（秒）
        public float spawnDelay = 0f;       // 这波开始前的等待时间
    }
}