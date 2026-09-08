using UnityEngine;
using System.Collections.Generic;

namespace MonsterSurvivors
{
    [CreateAssetMenu(fileName = "DropDatabase", menuName = "MonsterSurvivors/Drop Database")]
    public class DropDatabase : ScriptableObject
    {
        public List<DropData> drops = new List<DropData>();

        public DropData GetByType(DropType type)
        {
            return drops.Find(d => d.dropType == type);
        }
    }
}