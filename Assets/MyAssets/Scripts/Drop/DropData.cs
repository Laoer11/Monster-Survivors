using UnityEngine;

namespace MonsterSurvivors
{
    public enum DropType
    {
        Gem,       // 经验宝石
        Coin,      // 金币
        Food,      // 食物（回血）
        Magnet,    // 磁铁
        Bomb,      // 炸弹
        Chest,     // 宝箱
    }

    [CreateAssetMenu(fileName = "New Drop", menuName = "MonsterSurvivors/Drop Data")]
    public class DropData : ScriptableObject
    {
        public DropType dropType;
        public string dropName;
        public Sprite sprite;
        public GameObject prefab;       // 掉落物预制体
        public float value = 10;        // 数值（经验值/金币/回血量）
    }
}