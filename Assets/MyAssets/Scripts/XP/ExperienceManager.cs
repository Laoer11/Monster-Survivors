using UnityEngine;
using UnityEngine.Events;

namespace MonsterSurvivors
{
    public class ExperienceManager : MonoBehaviour
    {
        [SerializeField] private ExperienceData experienceData;

        public float CurrentXP { get; private set; }
        public int Level { get; private set; }
        public float TargetXP { get; private set; }

        /// <summary>
        /// 升级事件 —— 参数是新等级
        /// </summary>
        public UnityEvent<int> onLevelUp = new UnityEvent<int>();

        private void Awake()
        {
            TargetXP = experienceData.GetXP(0);
        }

        /// <summary>
        /// 增加经验，自动检测是否升级
        /// </summary>
        public void AddXP(float amount)
        {
            CurrentXP += amount;

            // 可能一次吃很多经验升很多级
            while (CurrentXP >= TargetXP)
            {
                CurrentXP -= TargetXP;
                Level++;
                TargetXP = experienceData.GetXP(Level);

                Debug.Log($"[XP] 升级！等级 {Level}，下一级需要 {TargetXP} XP");
                onLevelUp.Invoke(Level);
            }
        }
    }
}