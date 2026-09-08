using System;

namespace MonsterSurvivors
{
    [Serializable]
    public class CharactersSave : ISave
    {
        public string selectedCharacterId = "wizard";   // ★ 默认选中巫师

        public void Init()
        {
            selectedCharacterId = "wizard";
        }

        public void Flush() { }
    }
}
