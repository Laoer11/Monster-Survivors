using UnityEngine;
using System.Collections.Generic;

namespace MonsterSurvivors
{
    [CreateAssetMenu(fileName = "ExperienceData", menuName = "MonsterSurvivors/Experience Data")]
    public class ExperienceData : ScriptableObject
    {
        public List<ExperienceLevel> levels = new List<ExperienceLevel>();

        /// <summary>
        /// 获取升到某级所需经验
        /// </summary>
        public float GetXP(int level)
        {
            if (levels.Count == 0) return 10;
            if (level >= levels.Count) level = levels.Count - 1;
            return levels[level].xp;
        }
    }

    [System.Serializable]
    public class ExperienceLevel
    {
        public float xp;   // 从这一级升到下一级需要的经验
    }

}