using System.Collections.Generic;
using UnityEngine;

namespace MonsterSurvivors
{
    [CreateAssetMenu(fileName = "UpgradesDatabase", menuName = "Scriptable Objects/UpgradesDatabase")]
    public class UpgradesDatabase : ScriptableObject
    {
        public List<UpgradeData> upgrades = new List<UpgradeData>();

        public UpgradeData GetByType(UpgradeType type)
        {
            return upgrades.Find(u => u.upgradeType == type);
        }

        public int Count => upgrades.Count;
    }
}