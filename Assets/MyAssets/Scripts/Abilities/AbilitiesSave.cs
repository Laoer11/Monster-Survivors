using System;
using System.Collections.Generic;

namespace MonsterSurvivors
{
    [Serializable]
    public class AbilitiesSave : ISave
    {
        public List<string> acquiredTypes = new List<string>();
        public List<int> acquiredLevels = new List<int>();

        public void Init()
        {
            acquiredTypes.Clear();
            acquiredLevels.Clear();
        }

        public void Flush() {}
        
        public int GetLevel(AbilityType type)
        {
            string key = type.ToString();
            int index = acquiredTypes.IndexOf(key);
            return index >= 0 ? acquiredLevels[index] : -1;
        }

        public void SetLevel(AbilityType type, int level)
        {
            string key = type.ToString();
            int index = acquiredTypes.IndexOf(key);
            if(index >= 0)
            {
                acquiredLevels[index] = level;
            }
            else
            {
                acquiredTypes.Add(key);
                acquiredLevels.Add(level);
            }
        }

        public bool HasAbility(AbilityType type)
        {
            return acquiredTypes.Contains(type.ToString());
        }

        public int Count => acquiredTypes.Count;
    }
}