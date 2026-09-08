using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace MonsterSurvivors
{
    public class BossHealthBarUI : MonoBehaviour
    {
        public static BossHealthBarUI Instance { get; private set; }
        
        [SerializeField] private GameObject root;
        [SerializeField] private Image fillImage;
        [SerializeField] private TMP_Text nameText;
        [SerializeField] private TMP_Text hpText;

        private BossBehavior currentBoss;

        private void Awake()
        {
            Instance = this;
            root.SetActive(false);
        } 

        public static void Show(BossBehavior boss)
        {
            if(Instance == null) return;

            Instance.currentBoss = boss;
            Instance.root.SetActive(true);
            Instance.nameText.text = boss.Data.bossName;
        }

        public static void Hide()
        {
            if(Instance == null) return;
            Instance.root.SetActive(false);
            Instance.currentBoss = null;
        }

        private void Update()
        {
            if(currentBoss == null) return;

            fillImage.fillAmount = currentBoss.HPPercent;
            hpText.text = $"{Mathf.CeilToInt(currentBoss.CurrentHP)}/{Mathf.CeilToInt(currentBoss.MaxHP)}";
        }
    }
}