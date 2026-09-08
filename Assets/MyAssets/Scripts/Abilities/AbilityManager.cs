using System.Collections.Generic;
using UnityEngine;

namespace MonsterSurvivors
{
    public class AbilityManager : MonoBehaviour
    {
        [SerializeField] private AbilitiesDatabase database;

        private AbilitiesSave save;
        private int activeSlotsUsed;
        private int passiveSlotsUsed;

        public int ActiveSlotsUsed => activeSlotsUsed;
        public int PassiveSlotsUsed => passiveSlotsUsed;
        public int ActiveSlotsMax => database.activeSlotCount;
        public int PassiveSlotsMax => database.passiveSlotCount;

        private void Awake()
        {
            save = GameController.SaveManager.GetSave<AbilitiesSave>("AbilitiesSave");
            save.Init();
        }

        /// <summary>
        /// 升级时调用 —— 返回 3 个候选技能
        /// </summary>
        public List<AbilityData> GetCandidates()
        {
            var candidates = new List<AbilityData>();
            var pool = new List<AbilityData>();

            for(int i=0;i<database.Count;i++)
            {
                AbilityData ability = database.abilities[i];

                // 已满级 → 跳过
                if(save.GetLevel(ability.abilityType) >= ability.maxLevel) continue;

                // 进化技能先检查需求是否全部满足
                if (ability.isEvolution)
                {
                    bool allMet = true;
                    foreach (var req in ability.evolutionRequirements)
                    {
                        if (save.GetLevel(req.abilityType) < req.requiredLevel)
                        {
                            allMet = false;
                            break;
                        }
                    }
                    if (!allMet) continue;   // 需求没满足，不加入候选
                }


                pool.Add(ability);
            }

            // 加权随机选3个
            while(candidates.Count < 3 && pool.Count > 0)
            {
                AbilityData picked = WeightedPick(pool);
                candidates.Add(picked);
                pool.Remove(picked);
            }

            return candidates;
        }

        /// <summary>
        /// 玩家选了一个技能
        /// </summary>
        public void SelectAbility(AbilityType type)
        {
            int currentLevel = save.GetLevel(type);
            AbilityData data = database.GetByType(type);

            // 选了进化技能，先把被进化的旧技能标记为已进化
            if (data.isEvolution)
            {
                foreach (var req in data.evolutionRequirements)
                {
                    save.SetLevel(req.abilityType, 99);   // 99 表示"已被进化吸收"
                }
            }


            if (currentLevel == -1)
            {
                // 新技能：创建预制体
                save.SetLevel(type, 0);

                if (data.abilityPrefab != null)
                {
                    GameObject go = Instantiate(data.abilityPrefab);
                    DontDestroyOnLoad(go);
                    go.GetComponent<IAbilityBehavior>().Init(0);
                }

                if (data.isActiveAbility) activeSlotsUsed++;
                else passiveSlotsUsed++;
            }
            else
            {
                // 升级
                int newLevel = currentLevel + 1;
                save.SetLevel(type, newLevel);

                // 通知所有活跃技能，谁感兴趣谁处理
                var all = FindObjectsByType<MonoBehaviour>(FindObjectsSortMode.None);
                foreach (var m in all)
                {
                    if (m is IAbilityBehavior ability)
                        ability.Upgrade(newLevel);
                }

            }

            Debug.Log($"[AbilityManager] {type} 升至 Lv.{save.GetLevel(type)}");

        }

        public int GetLevel(AbilityType type) => save.GetLevel(type);
        public bool HasAbility(AbilityType type) => save.HasAbility(type);

        private AbilityData WeightedPick(List<AbilityData> pool)
        {
            float totalWeight = 0;
            foreach (var a in pool)
            {
                totalWeight += save.HasAbility(a.abilityType)
                    ? database.existingAbilityWeight
                    : database.newAbilityWeight;
            }

            float r = Random.Range(0, totalWeight);
            float progress = 0;

            foreach (var a in pool)
            {
                progress += save.HasAbility(a.abilityType)
                    ? database.existingAbilityWeight
                    : database.newAbilityWeight;
                if (r <= progress) return a;
            }

            return pool[pool.Count - 1];
        }
    }
}