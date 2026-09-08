using UnityEngine;

namespace MonsterSurvivors
{
    public interface IAbilityBehavior
    {
        void Init(int level);
        void Upgrade(int level);
    }
}

