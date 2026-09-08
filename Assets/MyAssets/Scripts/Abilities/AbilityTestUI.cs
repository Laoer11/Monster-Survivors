using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace MonsterSurvivors
{
    public class AbilityTestUI : MonoBehaviour
    {
        private AbilityManager abilityManager;
        private ExperienceManager experienceManager;

        [SerializeField] private GameObject panel;
        [SerializeField] private List<Button> choiceButtons;
        [SerializeField] private List<TMP_Text> choiceTexts;

        private List<AbilityData> currentCandidates;

        private void Start()
        {
            abilityManager = FindFirstObjectByType<AbilityManager>();
            experienceManager = FindFirstObjectByType<ExperienceManager>();

            // 订阅升级事件
            if (experienceManager != null)
                experienceManager.onLevelUp.AddListener(OnLevelUp);
        }


        // 开发者测试用：按 L 键手动触发
        private void Update()
        {
            // ★ 保留：按 L 键手动触发（开局没技能时用）
            if (UnityEngine.InputSystem.Keyboard.current != null &&
                UnityEngine.InputSystem.Keyboard.current.lKey.wasPressedThisFrame)
            {
                ShowChoices();
            }
        }

        // 升级时自动弹出
        private void OnLevelUp(int newLevel)
        {
            ShowChoices();
        }

        private void ShowChoices()
        {
            currentCandidates = abilityManager.GetCandidates();
            if (currentCandidates.Count == 0) return;

            Time.timeScale = 0;

            for (int i = 0; i < 3; i++)
            {
                if (i < currentCandidates.Count)
                {
                    choiceButtons[i].gameObject.SetActive(true);
                    choiceTexts[i].text = $"{currentCandidates[i].abilityName}\n{currentCandidates[i].description}";

                    int index = i;
                    choiceButtons[i].onClick.RemoveAllListeners();
                    choiceButtons[i].onClick.AddListener(() => Pick(index));
                }
                else
                {
                    choiceButtons[i].gameObject.SetActive(false);
                }
            }

            panel.SetActive(true);
            panel.GetComponent<CanvasGroup>().alpha = 0f;
            panel.GetComponent<CanvasGroup>().DoFade(1f, 0.2f);
        }

        private void Pick(int index)
        {
            abilityManager.SelectAbility(currentCandidates[index].abilityType);

            Time.timeScale = 1;

            panel.GetComponent<CanvasGroup>().DoFade(0f, 0.15f)
                .SetOnFinish(() => panel.SetActive(false));
        }
    }
}
