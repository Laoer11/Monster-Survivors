using UnityEngine;

namespace MonsterSurvivors
{
    /// <summary>
    /// 升级管理器 —— 管理永久升级的购买和效果
    /// </summary>
    public class UpgradesManager : MonoBehaviour
    {
        [SerializeField] private UpgradesDatabase database;

        private UpgradesSave save;

        private void Awake()
        {
            DontDestroyOnLoad(gameObject);
            save = GameController.SaveManager.GetSave<UpgradesSave>("UpgradesSave");
            GameController.RegisterUpgradesManager(this);
        }

        /// <summary>
        /// 获取某个升级项的当前等级
        /// </summary>
        public int GetLevel(UpgradeType type)
        {
            return save.GetLevel(type);
        }

        /// <summary>
        /// 尝试购买升级。成功返回 true
        /// </summary>
        public bool TryBuyUpgrade(UpgradeType type)
        {
            var data = database.GetByType(type);
            int currentLevel = save.GetLevel(type);

            if (currentLevel >= data.maxLevel) return false;

            int cost = data.GetCost(currentLevel);
            var playerSave = GameController.SaveManager.GetSave<PlayerSave>("PlayerSave");

            if (playerSave.gold < cost) return false;

            // 扣钱
            playerSave.gold -= cost;
            save.IncreaseLevel(type);

            GameController.SaveManager.SaveToDisk();
            Debug.Log($"[UpgradesManager] 购买 {type} 升至 Lv.{save.GetLevel(type)}，花费 {cost} 金币");
            return true;
        }

        /// <summary>
        /// 获取升级带来的属性加成值
        /// </summary>
        public float GetValue(UpgradeType type)
        {
            var data = database.GetByType(type);
            return data.GetValue(save.GetLevel(type));
        }

        /// <summary>
        /// 获取升级数据
        /// </summary>
        public UpgradeData GetData(UpgradeType type)
        {
            return database.GetByType(type);
        }
    }
}
