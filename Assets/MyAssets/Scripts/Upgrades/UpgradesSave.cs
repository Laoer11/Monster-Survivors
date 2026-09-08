using System;
using System.Collections.Generic;

namespace MonsterSurvivors
{
    [Serializable]
    public class UpgradesSave : ISave
    {
        // key = 升级类型的字符串，value = 当前等级
        public Dictionary<string, int> upgradeLevels = new Dictionary<string, int>();

        public void Init()
        {
            upgradeLevels.Clear();
        }

        public void Flush() { }

        public int GetLevel(UpgradeType type)
        {
            string key = type.ToString();
            return upgradeLevels.TryGetValue(key, out int val) ? val : 0;
        }

        public void IncreaseLevel(UpgradeType type)
        {
            string key = type.ToString();
            upgradeLevels[key] = GetLevel(type) + 1;
        }
    }
}
