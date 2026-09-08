using System.Collections.Generic;
using UnityEngine;

namespace MonsterSurvivors
{
    [CreateAssetMenu(fileName = "StagesDatabase", menuName = "MonsterSurvivors/StagesDatabase")]
    public class StagesDatabase : ScriptableObject
    {
        public List<StageData> stages = new List<StageData>();

        public StageData GetById(string id)
        {
            return stages.Find(stage => stage.stageId == id);
        }

        public int Count => stages.Count;   
    }
}